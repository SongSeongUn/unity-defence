using Battle.Skills;
using UnityEngine;

namespace Interface
{
    public interface IProjectileHitFeature
    {
        void Init(CurrentSkillData skillData);
        void OnHit(Vector2 hitPosition, BaseActor hitTarget);
    }
}