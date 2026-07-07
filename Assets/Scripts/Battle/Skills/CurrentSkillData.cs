using System;
using System.Collections.Generic;
using Actor;
using Actor.Features;
using Cysharp.Threading.Tasks;
using Table.Models;
using Table.TBL;
using UnityEngine;
using static Managers.DataTableManager;

namespace Battle.Skills
{
    public class SkillAbility
    {
        public int SkillCount;
        public int EffectCount;
        
        public int SkillDamage;
        public int EffectDamage;
        
        public int Speed;
        public float Cooldown;
        
        public int SkillDuration;
        public int EffectDuration;
        
        public int DotDamageInterval; // 도트 데미지 주기

        public int PierceRate; // 관통
        public float Scale; // 폭발 반경 등
        public int CCRate;
    }
    
    
    public class CurrentSkillData
    {
        public PlayerSkillRow SkillRow;
        public float NextSpawnTime = 0f;
        public HashSet<int> SkillUpgradeRows = new();
        public SkillAbility SkillAbility { get; private set; } = new ();
        
        public CurrentSkillData(PlayerSkillRow skillRow)
        {
            SkillRow = skillRow;
            SkillAbilityCalculrator.Calculate(SkillRow, SkillUpgradeRows, SkillAbility);
        }
        
        public void ReplaceUpgradeSkill(PlayerSkillRow skillRow, PlayerSkillUpgradeRow upgradeRow)
        {
            if (SkillUpgradeRows.Contains(upgradeRow.No))
            {
                DebugUtils.LogError($"Already have UpgradeSkills: {skillRow.No}");
                return;
            }

            if (upgradeRow.Skill_Requirement <= 0)
            {
                SkillUpgradeRows.Add(upgradeRow.No);
            }
            else
            {
                var group = GetTable<PlayerSKillUpgradeTable>()?.GetPlayerSkillUpgradeGroup(skillRow.No);

                if (group is null)
                    return;

                int beforeSkillUpgradeNo = 0;

                foreach (var row in group.Rows)
                {
                    if (SkillUpgradeRows.Contains(row.No))
                    {
                        beforeSkillUpgradeNo = row.No;
                        break;
                    }
                }

                if (beforeSkillUpgradeNo > 0)
                    SkillUpgradeRows.Remove(beforeSkillUpgradeNo);
                
                SkillUpgradeRows.Add(upgradeRow.No);
            }
            
            SkillAbilityCalculrator.Calculate(SkillRow, SkillUpgradeRows, SkillAbility);
        }
        
        public async UniTask InitWeaponPoolsAsync()
        {
            // TODO 이미 추가 됬다고 나오는 버그 있음 
            /**
             * ArgumentException: An item with the same key has already been added. Key: ChainLightning
             */


            try
            {
                if (SkillRow.Hit_Type == "CHAIN") // 체인라이트닝
                {
                    await ObjectPoolManager.Instance.InitObjectPool<ChainLightningActor>(
                        $"Projectile/{SkillRow.Prefab}.prefab", 5);
                }
                else
                {
                    await ObjectPoolManager.Instance.InitObjectPool<ProjectileActor>(
                        $"Projectile/{SkillRow.Prefab}.prefab", 3);
                }
            
                if (!string.IsNullOrWhiteSpace(SkillRow.Fx))
                    await ObjectPoolManager.Instance.InitObjectPool<FxActor>($"Fx/{SkillRow.Fx}.prefab", 3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }
    }
}