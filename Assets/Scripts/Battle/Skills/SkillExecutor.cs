using UnityEngine;
using Events;
using Events.Player;

namespace Battle.Skills
{
    public abstract class SkillExecutor : MonoBehaviour
    {
        public abstract void ExecuteSkill(CurrentSkillData skillData, Transform target, Transform firePoint);
        
        protected virtual T Get<T>(CurrentSkillData skillData, Vector2 position) where T : Component
        {
            return ObjectPoolManager.Instance.Get<T>(skillData.SkillRow.Prefab, position);
        }
    }
}