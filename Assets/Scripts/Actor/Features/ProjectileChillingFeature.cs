using Battle.Skills;
using Interface;
using UnityEngine;


namespace Actor.Features
{
    public class ProjectileChillingFeature : MonoBehaviour, IProjectileHitFeature
    {
        [Header("타겟 레이어")]
        [SerializeField] private LayerMask _targetLayer;
        
        private int _damage;
        private string _fxPrefab;
        private int _slowRate;
        private int _slowDuration;
        
        public void Init(CurrentSkillData skillData)
        {
            _damage = skillData.SkillAbility.SkillDamage;
            _fxPrefab = skillData.SkillRow.Fx;
            _slowRate = skillData.SkillAbility.CCRate;
            _slowDuration = skillData.SkillRow.Effect_Duration;
        }
        
        public void OnHit(Vector2 hitPosition, BaseActor hitTarget)
        {
            hitTarget.TakeDamage(_damage); // 타격 데미지
            
            // 슬로우
            if (hitTarget is MonsterActor monsterActor)
                monsterActor.OnMonsterSlow(_slowRate,  _slowDuration);
            
            _ = PlayFx(hitPosition);
        }
        
        private FxActor PlayFx(Vector2 position)
        {
            if (string.IsNullOrWhiteSpace(_fxPrefab) || ObjectPoolManager.Instance is null)
                return null;

            FxActor fx = ObjectPoolManager.Instance.Get<FxActor>(_fxPrefab, position);
            fx?.Init(_fxPrefab);
            return fx;
        }
    }
}