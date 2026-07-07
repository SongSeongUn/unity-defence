using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIWindow
{
    public class UI_IntroScene_Page : UIPage
    {
        [Header("UI_IntroScene_Page")]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _exiteButton;
        
        protected override void Awake()
        {
            base.Awake();
        }
        
        public void AddButtonsEvents(Action onStartEvent, Action onExitEvent)
        {
            _startButton.onClick.AddListener(() => onStartEvent?.Invoke());
            _exiteButton.onClick.AddListener(() => onExitEvent?.Invoke());
        }
    }
}