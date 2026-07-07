using Cysharp.Threading.Tasks;
using Battle.Skills;
using Events;
using Events.Player;
using UnityEngine;
using Managers;

namespace Controller
{
    public class SkillSelectionController : MonoBehaviour
    {
        [SerializeField] SkillInventoryController _skillInventoryController;

        private UI.Popup.UI_SkillSelect_Popup _uiSelectPopup;
        private void OnEnable()
        {
            GameEvents.AddListener<PlayerLevelUpEvent>(OnPlayerLevelUp);
            GameEvents.AddListener<PlayerSkillSelecdtEvent>(OnSkillSelected);
        }

        private void OnDisable()
        {
            GameEvents.RemoveListener<PlayerLevelUpEvent>(OnPlayerLevelUp);
            GameEvents.RemoveListener<PlayerSkillSelecdtEvent>(OnSkillSelected);
        }
        
        private void OnPlayerLevelUp(PlayerLevelUpEvent evt)
        {
            ShowLevelUpSkillSelect().Forget();
        }

        private void OnSkillSelected(PlayerSkillSelecdtEvent evt)
        {
            _skillInventoryController.ApplySelectedSkillAsync(evt.SkillRow, evt.UpgradeRow, () =>
            {
                Time.timeScale = 1f;
                _uiSelectPopup.Close();
            }).Forget();
        }
        
        private async UniTaskVoid ShowLevelUpSkillSelect()
        {
            var candidates = Core.SkillUpgradeSelector.Pick(
                _skillInventoryController.CurrentSkillsRows
                , _skillInventoryController.CurrentSKillUpgradeRows
                , 3);

            Time.timeScale = 0f; // 타임

            _uiSelectPopup = await UIManager.Instance.OpenUI<UI.Popup.UI_SkillSelect_Popup>("UI_SkillSelect_Popup");
            _uiSelectPopup.Show(candidates, _skillInventoryController.CurrentSkillsRows);
            
        }
    }
}