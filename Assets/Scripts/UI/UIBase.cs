using System;
using Managers;
using UnityEngine;

namespace UI
{
    public enum UIWindowType
    {
        Page,
        Popup,
        WorldCanvas,
    }
    
    public abstract class UIBase : MonoBehaviour
    {
        public UIWindowType UIType;
        private Canvas _canvas;
        public Canvas Canvas 
        {
            get
            {
                if(_canvas == null) TryGetComponent(out _canvas);
                return _canvas;
            }  
        }
        
        public int SortOrder { get { return _canvas.sortingOrder; } set { _canvas.sortingOrder = value; }}
        
        public virtual void Refresh() { }

        public virtual void Open() => gameObject.SetActive(true);
        public virtual void Close() => gameObject.SetActive(false);

        protected virtual void Awake()
        {
            gameObject.layer = LayerMask.NameToLayer("UI");;
        }

        public virtual void Init(Camera uiCamera, RenderMode renderMode)
        {
            Canvas.renderMode = renderMode;
            Canvas.worldCamera = uiCamera;
        }
    }

    public class UIPage : UIBase
    {
        
    }

    public class UIPopup : UIBase
    {
        
    }

    public class UIWorldCanvas : UIBase
    {
        
    }
}