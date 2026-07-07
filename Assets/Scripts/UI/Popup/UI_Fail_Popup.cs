using Events;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Popup
{
    public class UI_Fail_Popup : UIPopup
    {
        [SerializeField] private Button _button;

        private void Awake()
        {
            _button.onClick.AddListener(OnGoToIntroScene);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
        
        private async void OnGoToIntroScene()
        {
            GameEvents.MoveSceneRemoveAllListner();
            Time.timeScale = 1;
            await SceneControllManager.Instance.LoadScene("IntroScene");
        }
    }
}