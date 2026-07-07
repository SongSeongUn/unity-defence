using UnityEngine;
using Common;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.Rendering;
using UI;
using UnityEngine.Rendering.Universal;

namespace Managers
{
    public class UIManager : MonoSceneSingleton<UIManager>
    {
        [Header("UI Manager")]
        [SerializeField] private SerializedDictionary<UIWindowType, Transform> uiRoot;
        private Dictionary<Type, UIBase> UIs = new ();
        
        [SerializeField] private Camera _uiCamera;
        public Camera UICamera { get => _uiCamera; }
        
        private Camera _mainCamera;
        public Camera MainCamera { get => _mainCamera; }

        private static bool _isCreate = false;
        
        protected override void Awake()
        {
            base.Awake();

            if (!_isCreate) return;
            FindCamera();
        }

        private void FindCamera()
        {
            _mainCamera = Camera.main;
            
            _mainCamera.GetUniversalAdditionalCameraData().cameraStack.Add(UICamera);
        }

        public static async UniTask Create()
        {
            if (!_isCreate)
            {
                var go = await AddressableManager.Instance.LoadAssetAsync<GameObject>("UI/@UIRoot.prefab");
                var uiManager = Instantiate(go.GetComponent<UIManager>());
                uiManager.FindCamera();
                _isCreate = true;
            }
        }
        
        public async UniTask<T> OpenUI<T>(string name, int sortOrder = 1)  where T : UIBase
        {
            if (UIs.TryGetValue(typeof(T), out var ui))
            {
                ui.SortOrder = sortOrder;
                return ui as T;
            }
            else
            {
                T createUI =  await CreatUI<T>(name);
                if (createUI == null)
                    return null;
                
                if (createUI.UIType == UIWindowType.WorldCanvas)
                    createUI.Init(_uiCamera, RenderMode.ScreenSpaceOverlay);
                else
                    createUI.Init(_uiCamera, RenderMode.ScreenSpaceCamera);
                
                UIs.Add(typeof(T), createUI);
                SetUIParentRoot(createUI);
                
                createUI.SortOrder =  sortOrder;
                createUI.Canvas.sortingLayerName = "UI";
                return createUI;
            }
        }

        private async UniTask<T> CreatUI<T>(string name) where T : UIBase
        {
            var path = $"UI/{name}.prefab";
            var prefab = await AddressableManager.Instance.LoadAssetAsync<GameObject>(path);
            var go = Instantiate(prefab, transform);
            go.TryGetComponent(out T ui);
            return ui;
        }
        
        private void SetUIParentRoot(UIBase ui)
        {
            uiRoot.TryGetValue(ui.UIType, out var parent);
            if (parent != null) ui.transform.SetParent(parent);
        }

        public void ReleaseUI(UIBase ui)
        {
            Type type = ui.GetType();
            if (!UIs.TryGetValue(type, out var _ui))
                return;

            string assetPath = $"UI/{type.Name}.prefab";
            
            UIs.Remove(type);
            Destroy(ui.gameObject);
            AddressableManager.Instance.ReleaseAsset(assetPath);
        }


        protected override void OnDestroy()
        {
            _isCreate = false;
            base.OnDestroy();
            // AddressableManager.Instance.ReleaseAsset("UI/@UIRoot.prefab");
        }
    }
}