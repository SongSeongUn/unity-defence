using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.U2D;

public class AddressableManager : MonoBehaviour
{
    private static AddressableManager _instance;
    public static AddressableManager Instance => _instance;

    private readonly Dictionary<string, AsyncOperationHandle> _handlesDic = new();
    // 중복 캐스팅 문제 회피
    private readonly Dictionary<string, AsyncLazy<UnityEngine.Object>> _loadingLazyTasks = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        if (_instance != null)
            return;

        GameObject go = new("AddressableManager (Auto)");
        _instance = go.AddComponent<AddressableManager>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private async void LoadAtlasAsync(string name, string path, Action<SpriteAtlas> completeAction)
    {
        try
        {
            SpriteAtlas atlas = await LoadAssetAsync<SpriteAtlas>(path);
            if (atlas == null)
                return;

            completeAction.Invoke(atlas);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError($"{name} {path} load failed");
        }
    }

    public async UniTask<T> LoadAssetAsync<T>(string path) where T : UnityEngine.Object
    {
        if (_handlesDic.TryGetValue(path, out AsyncOperationHandle handle))
        {
            if (handle.IsValid())
                return handle.Result as T;

            _handlesDic.Remove(path);
        }
        
        
        if (!_loadingLazyTasks.TryGetValue(path, out var lazy))
        {
            // 비동기 초기화 작업이 중복 실행되지 않도록 보호
            lazy = new AsyncLazy<UnityEngine.Object>(() => LoadAssetInternalAsync<T>(path));
            _loadingLazyTasks.Add(path, lazy);
        }


        var result = await lazy;
        return result as T;
    }

    private async UniTask<UnityEngine.Object> LoadAssetInternalAsync<T>(string path)
        where T : UnityEngine.Object
    {
        try
        {
            var handle = Addressables.LoadAssetAsync<T>(path);
            var result = await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded && result != null)
            {
                _handlesDic[path] = handle;
                return result;
            }

            if (handle.IsValid())
                Addressables.Release(handle);

            return null;
        }
        catch(Exception e)
        {
            DebugUtils.LogError($"[AddressableManager] Exception: {path}\n{e.Message}");
            return null;
        }
        finally
        {
            _loadingLazyTasks.Remove(path);
        }
    }


    public void ReleaseAsset(string key)
    {
        if (!_handlesDic.TryGetValue(key, out AsyncOperationHandle handle))
            return;

        Addressables.Release(handle);
        _handlesDic.Remove(key);
        Debug.Log($"[AddressableManager] Released: {key}");
    }

    private void OnDestroy()
    {
        foreach (var handle in _handlesDic.Values)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

        _handlesDic.Clear();
    }
}