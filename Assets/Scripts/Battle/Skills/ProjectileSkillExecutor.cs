using Cysharp.Threading.Tasks;
using Interface;
using UnityEngine;

namespace Battle.Skills
{
    public class ProjectileSkillExecutor : SkillExecutor
    {
        public async override void ExecuteSkill(CurrentSkillData skillData, Transform target, Transform firePoint)
        {
            // 투사체 개수 증가
            for (int i = 0; i < skillData.SkillAbility.SkillCount; i++)
            {
                if (i > 0)
                    await UniTask.Delay(200);
                var projectile = Get<ProjectileActor>(skillData, firePoint.position);
                
                if (projectile == null)
                    return;
                
                projectile.Init(skillData, target.transform, firePoint);
            }
        }
    }
}