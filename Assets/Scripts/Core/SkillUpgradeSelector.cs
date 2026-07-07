using System.Collections.Generic;
using System.Linq;
using Table.Models;
using UnityEngine;

using static Managers.DataTableManager;
using Table.TBL;

namespace Core
{
    public static class SkillUpgradeSelector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentSkillRows"></param>
        /// <param name="currentSkillUpgradeRows"></param>
        /// <param name="count"></param>
        /// <returns>no, Skill_No | true : 신규, false : 업그레이드/returns>
        public static List<(int, bool)> Pick(
            IReadOnlyCollection<int> currentSkillRows,
            IReadOnlyCollection<int> currentSkillUpgradeRows,
            int count)
        {
            var rows = GetTable<PlayerSKillUpgradeTable>()?.Rows;
            if (rows is null || rows.Count <= 0)
                return null;
            
            List<(int, bool)> results = new();
            
            List<PlayerSkillUpgradeRow> candidates = rows
                .Where(row => row != null)
                .Where(row => row.Skill_Chance > 0) // 확률 확인
                .Where(row => IsRequirementSatisfied(row, currentSkillRows, currentSkillUpgradeRows)) // 선행 만족
                .Where(row => currentSkillUpgradeRows == null || !currentSkillUpgradeRows.Contains(row.No)) // 중복 확인
                .ToList();

            while (results.Count < count && candidates.Count > 0)
            {
                PlayerSkillUpgradeRow selected = PickWeighted(candidates);
                if (selected == null)
                    break;

                if (selected.Hit_Type != "SELF" && !currentSkillRows.Contains(selected.Skill_No))
                {
                    // 최초 획득해야하는 스킬 항목들 전부 제거
                    results.Add((selected.Skill_No, true));
                    candidates.RemoveAll(row => row.Skill_No == selected.Skill_No);
                }
                else
                {
                    results.Add((selected.No, false));
                    candidates.Remove(selected);
                }
            }

            return results;
        }

        private static bool IsRequirementSatisfied(
            PlayerSkillUpgradeRow row,
            IReadOnlyCollection<int> currentSkillRows,
            IReadOnlyCollection<int> currentSkillUpgradeRows)
        {
            if (row.Hit_Type == "SELF") return true;

            if (!currentSkillRows.Contains(row.Skill_No))
                return row.Skill_Requirement <= 0;
            
            return row.Skill_Requirement <= 0
                   || currentSkillUpgradeRows != null && currentSkillUpgradeRows.Contains(row.Skill_Requirement);
        }

        private static PlayerSkillUpgradeRow PickWeighted(List<PlayerSkillUpgradeRow> candidates)
        {
            int totalChance = candidates.Sum(row => Mathf.Max(0, row.Skill_Chance));
            if (totalChance <= 0)
                return null;

            int roll = Random.Range(1, totalChance + 1);
            int accumulated = 0;

            foreach (PlayerSkillUpgradeRow row in candidates)
            {
                accumulated += Mathf.Max(0, row.Skill_Chance);
                if (roll <= accumulated)
                    return row;
            }

            return candidates[^1];
        }
    }
}
