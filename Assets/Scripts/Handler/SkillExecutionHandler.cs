using Battle.Skills;
using Common;
using Interface;
using UnityEngine;

namespace Handler
{
    public class SkillExecutionHandler : MonoBehaviour
    {
        [SerializeField] private ProjectileSkillExecutor _projectileSkillExecutor;
        [SerializeField] private ChainSkillExecutor _chainSkillExecutor;
        
        public void ExecuteSkill(CurrentSkillData skillData, Transform target, Transform firePoint)
        {
            if (skillData is null) return;
            
            SkillHitType type = EnumParser.ParseHitType(skillData.SkillRow.Hit_Type);
            switch (type)
            {
                case SkillHitType.Single:
                    _projectileSkillExecutor.ExecuteSkill(skillData, target, firePoint);
                    break;
                case SkillHitType.Chain:
                    _chainSkillExecutor.ExecuteSkill(skillData, target, firePoint);
                    break;
                default:
                    break;
            }
        }
    }
}