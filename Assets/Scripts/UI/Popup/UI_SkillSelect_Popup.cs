using System;
using System.Collections.Generic;
using System.Linq;
using Table.Models;
using UnityEngine;

using UI.UIItem;

namespace UI.Popup
{
    public class UI_SkillSelect_Popup : UIPopup
    {
        [SerializeField] private List<UI_SkillSelectSlot> skillSelectSlots = new();
        public void Show(List<(int, bool)> candidates, IReadOnlyCollection<int> runtimeSkillRows)
        {
            if (candidates.Count == 0)
            {
                Time.timeScale = 1f;
                DebugUtils.LogError("스킬 개수 0개");
                return;
            }

            if (candidates.Count < 3)
            {
                // TODO 혹시 3개 안되고 두개면 회복이나 다른 스킬 추가
                DebugUtils.Log($"후보군 3개 안됨 : {candidates.Count}개");
                return;
            }
                
            for (int i = 0; i < candidates.Count; i++)
            {
                if (candidates[i].Item2) // 신규 스킬
                {
                    skillSelectSlots[i].ShowNewSkill(candidates[i].Item1);
                }
                else // 스킬 업그레이드
                {
                    skillSelectSlots[i].ShowUpgradeSkill(candidates[i].Item1);
                }
            }
            
            Open();
        }
    }
}