using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Cysharp.Threading.Tasks; // UniTask 필수


// OdinInspector 필요
// UniTask 필수

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public string Tag;          // 태그 이름
        public int CreateMinSize = 10;       // 초기 생성 개수
        public GameObject _prefab;

        private void UpdateTagName()
        {
            if (_prefab != null)
            {
                Tag = _prefab.name; // 프리팹 이름을 태그로 쏙!
            }
            else
            {
                Tag = ""; // 프리팹 빼면 태그도 비움
            }
        }
    }

    public List<Pool> Pools;
    private Dictionary<string, Queue<Component>> _poolDictionary;

    // 부모 정리용 (Hierarchy 깔끔하게)
    private Transform _poolParentRoot;

    // 어드레서블 핸들 관리

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeInspectorPool();
    }

    
    private void InitializeInspectorPool()
    {
        _poolDictionary = new Dictionary<string, Queue<Component>>();

        // 하이에라키 지저분해지지 않게 부모 하나 생성
        GameObject rootObj = new ("/@ObjectPool_Root");
        rootObj.transform.SetParent(transform);
        _poolParentRoot = rootObj.transform;

        # region 하이라키 사용(단순 오브젝트 생성용)
        foreach (Pool pool in Pools)
        {
            // 태그별로 정리할 자식 오브젝트 생성
            CreatePoolInternal<Transform>(pool.Tag, pool._prefab, pool.CreateMinSize);
        }
        #endregion
    }


    # region 하이라키 사용(단순 오브젝트 생성용)
    private void CreatePoolInternal<T>(string tag, GameObject prefab, int count) where T : Component
    {
        if (_poolDictionary.ContainsKey(tag)) return;

        Queue<Component> queue = new ();
        
        // 하이에라키 정리용 부모
        GameObject subPoolObj = new ($"Pool_{tag}");
        subPoolObj.transform.SetParent(_poolParentRoot);

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, subPoolObj.transform);
            obj.name = $"{tag}_{i:000}";
            obj.SetActive(false);

            // [핵심] 생성 시점에 T 컴포넌트를 찾아서 저장 (캐싱)
            // 만약 T가 Transform이면 그냥 transform을 저장
            
            if (!obj.TryGetComponent<T>(out var component))
            {
                Debug.LogError($"[ObjectPool] {prefab.name}에 {typeof(T)} 컴포넌트가 없습니다!");
                Destroy(obj);
                continue; // 실패하면 넣지 않음
            }

            queue.Enqueue(component);
        }

        _poolDictionary.Add(tag, queue);
    }

    #endregion


    // === 외부에서 호출: 오브젝트 꺼내기 ===
    public T Get<T>(string tag, Vector3 position /*, Quaternion rotation*/) where T : Component
    {
        if (!_poolDictionary.TryGetValue(tag, out var queue))
        {
            DebugUtils.LogError($"[ObjectPool] '{tag}' 태그를 찾을 수 없습니다! 인스펙터에 등록했나요?");
            return default;
        }

        T resultComponent = null;

        if (queue.Count > 0)
        {
            resultComponent = queue.Dequeue() as T;
        }
        else
        {
            var poolInfo = Pools.Find(p => p.Tag == tag);
            if (poolInfo != null)
            {
                GameObject obj = Instantiate(poolInfo._prefab, _poolParentRoot.Find($"Pool_{tag}"));
                obj.name = $"{tag}_Extra";
                resultComponent = obj.GetComponent<T>();
            }
        }

        // 3. 초기화 및 리턴
        if (resultComponent != null)
        {
            resultComponent.transform.position = position;
            resultComponent.gameObject.SetActive(true);
            
            return resultComponent;
        }

        return null;
    }

    public void Return<T>(string tag, T obj) where T : Component
    {
        if (!_poolDictionary.TryGetValue(tag, out Queue<Component> queue))
            return;

        obj.gameObject.SetActive(false);
        queue.Enqueue(obj); // 다 썼으니 다시 대기열(큐)에 넣음
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="createMinSize"> -1 기본</param>
    /// <returns></returns>
    public async UniTask InitObjectPool<T>(string path, int createMinSize = 0) where T : Component
    {
        string tag = System.IO.Path.GetFileNameWithoutExtension(path); // 프리팹 이름

        if (_poolDictionary.ContainsKey(tag)) return;


        GameObject prefab = await AddressableManager.Instance.LoadAssetAsync<GameObject>(path);
        if (prefab == null) return;

        Pool newPoolInfo = new() ;
        newPoolInfo.Tag = tag;
        newPoolInfo._prefab = prefab;
        if (createMinSize > 0) newPoolInfo.CreateMinSize = createMinSize;

        Pools.Add(newPoolInfo);

        Queue<Component> queue = new ();
        GameObject subPoolObj = new ($"Pool_{tag}");
        subPoolObj.transform.SetParent(_poolParentRoot);

        for (int i = 0; i < newPoolInfo.CreateMinSize; i++)
        {
            GameObject obj = Instantiate(prefab, subPoolObj.transform);
            obj.name = $"{tag}_{i:000}";
            obj.SetActive(false);

            if(obj.TryGetComponent<T>(out var component)) queue.Enqueue(component);

            // [최적화] 5개 만들 때마다 한 프레임 쉬기 (렉 방지)
            if (i % 5 == 0) await UniTask.Yield();
        }

        _poolDictionary.Add(tag, queue);
    }

    // private async UniTask<GameObject> LoadPrefab(string path)
    // {
    //     try
    //     {
    //         // 핸들 관리 등을 생략한 간단한 로드입니다. 실제 프로젝트에선 핸들 관리가 필요할 수 있습니다.
    //         return await Addressables.LoadAssetAsync<GameObject>(path).ToUniTask();
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError($"Load Failed: {path} / {e.Message}");
    //         return null;
    //     }
    // }
}