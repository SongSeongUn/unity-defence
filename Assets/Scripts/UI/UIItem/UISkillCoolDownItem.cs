using Common;
using Cysharp.Threading.Tasks;
using Table.Models;
using Table.TBL;
using UnityEngine;
using UnityEngine.UI;

using static Managers.DataTableManager;


namespace UI.UIItem
{
    public class UISkillCoolDownItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Image _coolDownImage; 
        
        [SerializeField] private TextExtension _LevelTxt;
        
        public int SkillNo { get; private set; }
        public bool IsActive => gameObject.activeSelf && SkillNo > 0;

        private float _coolDownEndTime;
        private float _coolDownDuration;

        public void Show(int skillNo)
        {
            var skillRow = GetTable<PlayerSkillTable>()?.GetData(skillNo);
            if (skillRow == null)
                return;

            SkillNo = skillRow.No;
            //SetLevel(level);
            gameObject.SetActive(true);
            LoadIconAsync(skillRow.Skill_Icon).Forget();
        }

        public void StartCoolDown(float coolDownEndTime, float duration)
        {
            _coolDownEndTime = coolDownEndTime;
            _coolDownDuration = Mathf.Max(0.1f, duration);
            enabled = true;
        }

        private void Update()
        {
            float remain = _coolDownEndTime - Time.time;
            if (remain <= 0f)
            {
                _coolDownImage.fillAmount = 0f;
                enabled = false;
                return;
            }
            
            _coolDownImage.fillAmount = Mathf.Clamp01(remain / _coolDownDuration);
        }

        private void SetLevel(int level)
        {
            if (_LevelTxt != null)
                _LevelTxt.TextValue = $"{level}";
        }

        private async UniTaskVoid LoadIconAsync(string iconName)
        {
            if (_image == null || string.IsNullOrWhiteSpace(iconName))
                return;

            Sprite sprite = await AddressableManager.Instance.LoadAssetAsync<Sprite>($"Resource/Skill/{iconName}.png");
            if (sprite != null)
            {
                _image.sprite = sprite;
                _image.enabled = true;
            }
        }
    }
}
