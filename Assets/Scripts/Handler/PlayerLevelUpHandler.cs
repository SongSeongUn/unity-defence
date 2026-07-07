using UnityEngine;
using Events;
using Events.Player;
using Table.Models;
using Table.TBL;
using static Managers.DataTableManager;

namespace Handler
{
    public class PlayerLevelUpHandler : MonoBehaviour
    {
        [SerializeField] StageController _stagecontroller;
        
        private bool _isLevelUpSelecting;
        
        [Header("현재 레벨")]
        private int _currentLevel = 1;
        
        
        [Header("누적 경험치")]
        public int TotalExp { get; set; } = 0;
        
        public void AddExp(MonsterActor mon)
        {
            var expRow = GetTable<BattleExpTable>()?.GetData(_currentLevel);
            if (expRow == null)
            {
                DebugUtils.LogError("Exp Row Not Found");
                return;
            }
            
            int gainedExp = mon.Grade switch
            {
                MonsterGrade.Normal     => expRow.Nomal_Exp, 
                MonsterGrade.MiddleBoss => expRow.Mid_Boss_Exp,
                MonsterGrade.Boss       => expRow.Boss_Exp,
                _                       => 0
            };
            
            
            if (gainedExp <= 0)
                return;

            TotalExp += gainedExp;
            _stagecontroller.RefreshExpUI(TotalExp, _currentLevel);

            if (!_isLevelUpSelecting)
                TryLevelUp();
        }

        private void TryLevelUp()
        {
            // 레벨업
            BattleExpRow expRow = GetTable<BattleExpTable>()?.GetData(_currentLevel);
            if (expRow == null || TotalExp < expRow.Need_Exp)
                return;

            TotalExp -= expRow.Need_Exp;
            GameEvents.SendEvent(new PlayerLevelUpEvent(_currentLevel, ++_currentLevel));

            
            _stagecontroller.RefreshExpUI(TotalExp, _currentLevel);
        }
    }
}