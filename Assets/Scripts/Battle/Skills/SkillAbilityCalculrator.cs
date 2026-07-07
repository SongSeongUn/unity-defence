using System;
using System.Collections.Generic;
using Common;
using Managers;
using Table.Models;
using Table.TBL;
using UnityEngine;

namespace Battle.Skills
{
    public static class SkillAbilityCalculrator
    {
        private static void Init(PlayerSkillRow skill, SkillAbility ability)
        {
            ability.SkillCount = skill.Skill_Count;
            ability.EffectCount = skill.Effect_Count;
            ability.Speed = skill.Skill_Speed;
            ability.Cooldown = skill.Cooldown;
            ability.SkillDamage = skill.Skill_Damage;
            ability.EffectDamage = skill.Effect_Damage;
            ability.EffectDuration = skill.Effect_Duration;
            ability.DotDamageInterval = skill.Dot_Damage_Inteval;
            ability.CCRate = skill.CC_Rate;
            ability.Scale = 1;
            ability.PierceRate = 0;
        }
        
        public static void Calculate(PlayerSkillRow skill, IReadOnlyCollection<int> skillUpgrades, SkillAbility ability)
        {
            if (skill is null)
            {
                DebugUtils.LogError("Skill 정보 없음");
                return;
            }

            var table = DataTableManager.GetTable<PlayerSKillUpgradeTable>();
            if (table is null)
            {
                DebugUtils.LogError($"PlayerSkillUpgrade Table is null");
                return;
            }
            
            Init(skill, ability);
            
            foreach (var skillUpgrade in skillUpgrades)
            {
                var upgradeRow = table.GetData(skillUpgrade);
                
                if (upgradeRow is null)
                {
                    DebugUtils.LogError($"PlayerSkillUpgrade Table Data Not Found No is {skillUpgrade}");
                    return;
                }

                SkillUpgradeType type = EnumParser.ParseUpgradeType(upgradeRow.SKILL_UPGRADE_TYPE);
                
                switch (type)
                {
                    case SkillUpgradeType.DamageUpgrade:
                        ability.SkillDamage += Mathf.RoundToInt((float)skill.Skill_Damage * upgradeRow.Skill_Damage_Add_Rate / 100000);
                        
                        ability.EffectDamage += Mathf.RoundToInt((float)skill.Effect_Damage * upgradeRow.Effect_Damage_Add_Rate / 100000);
                        break;
                    case SkillUpgradeType.SpawnUpgrade:
                        ability.SkillCount += upgradeRow.Skill_Add_Count;
                        ability.EffectCount += upgradeRow.Effect_Add_Count;
                        break;
                    case SkillUpgradeType.HitCountUpgrade:
                        ability.EffectCount += upgradeRow.Effect_Add_Count;
                        break;
                    case SkillUpgradeType.CooldownUpgrade:
                        ability.Cooldown -= skill.Cooldown * upgradeRow.Cooldown_Sub_Rate / 100000f;
                        // 최소 쿨타임 있으면 구현
                        break;
                    case SkillUpgradeType.PierceUpgrade:
                        ability.PierceRate += upgradeRow.Stat_Add_Rate;
                        break;
                    case SkillUpgradeType.RangeUpgrade:
                        ability.Scale += upgradeRow.Stat_Add_Rate / 1000f;
                        break;
                    case SkillUpgradeType.CCUpgrade:
                        ability.CCRate += upgradeRow.CC_Add_Rate;
                        break;
                    case SkillUpgradeType.DurationUpgrade:
                        ability.EffectDuration += upgradeRow.Effect_Add_Duration;
                        break;
                }
            }
        }
    }
}