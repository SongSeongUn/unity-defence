using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Events;
using Events.Player;
using Table.Models;
using Table.TBL;
using UI.UIItem;
using UnityEngine;
using UnityEngine.UI;

using static Managers.DataTableManager;

namespace UI.UIWindow
{
    public class UIStageData
    {
        public int Stage;
        public int Wave;
    }
    
    public class UI_MainStage_Page : UIPage
    {
        [Header("UI_MainStage_Page")]
        [SerializeField] private Slider _LevelSlider;
        
        [SerializeField] private TextExtension _stageNameTxt;

        [SerializeField] private TextExtension _WaveTimerTxt;

        [SerializeField] private List<UISkillCoolDownItem> SkillList;
        

        private UIStageData _CurrentStageData;
        private float _waveTimer;

        private void OnEnable()
        {
            GameEvents.AddListener<WaveStartEvent>(OnRefreshWave);
            // Weapon 데이터셋 
            GameEvents.AddListener<PlayerSkillCoolDownEvent>(OnRefreshSKillCoolDown);
            GameEvents.AddListener<PlayerSkillEquippedEvent>(OnRefreshNewSkill);
        }

        private void OnDisable()
        {
            GameEvents.RemoveListener<WaveStartEvent>(OnRefreshWave);
            // Weapon 데이터셋 
            GameEvents.RemoveListener<PlayerSkillCoolDownEvent>(OnRefreshSKillCoolDown);
            GameEvents.RemoveListener<PlayerSkillEquippedEvent>(OnRefreshNewSkill);
        }

        public void Show(UIStageData stageData)
        {
            base.Open();
            _CurrentStageData = stageData;
            
            var battleMapRow = GetTable<BattleMapTable>()?.GetData(_CurrentStageData.Stage);
            if (battleMapRow == null) throw new Exception("BattleMapData is Null");
            
            _stageNameTxt.TextValue = string.Format("Stage {0}", stageData.Stage);
            //HideSkillSelectSlots();
            RefreshExp(0, 1);
        }

        public void RefreshExp(int currentExp, int needExp)
        {
            if (_LevelSlider == null)
                return;

            _LevelSlider.minValue = 0f;
            _LevelSlider.maxValue = Mathf.Max(1, needExp);
            _LevelSlider.value = Mathf.Clamp(currentExp, 0, Mathf.Max(1, needExp));
        }
        
        private void OnRefreshWave(WaveStartEvent evt)
        {
            _waveTimer = evt.Timer;
        }

        private void OnRefreshSKillCoolDown(PlayerSkillCoolDownEvent evt)
        {
            var weapon = SkillList.Find(weapon => weapon != null && weapon.SkillNo == evt.WeaponNo);
            if (weapon is null) return;
            
            weapon.StartCoolDown(evt.CoolDownTime, evt.Duration);
        }

        private void OnRefreshNewSkill(PlayerSkillEquippedEvent evt)
        {
            UISkillCoolDownItem skill = SkillList.Find(item => item != null && item.SkillNo == evt.SkillNo);
            if (skill == null)
                skill = SkillList.Find(item => item != null && !item.IsActive);

            if (skill == null)
                return;

            skill.Show(evt.SkillNo);
        }

       
        private void Update()
        {
            // 웨이브 타이머 설정
            int time = (int)Math.Max(_waveTimer - Time.time, 0f);
            _WaveTimerTxt.TextValue = string.Format("{0}", time);;
        }
    }
}
