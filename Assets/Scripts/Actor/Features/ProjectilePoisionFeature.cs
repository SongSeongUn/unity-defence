using UnityEngine;
using Interface;
using Battle.Skills;

namespace Actor.Features
{
    public class ProjectilePoisionFeature : MonoBehaviour, IProjectileHitFeature
    {
        [Header("타겟 레이어")]
        [SerializeField] private LayerMask _targetLayer;
        
        private int _damage;
        private int _effectDamage;
        private int _duration;
        private int _tickInterval;
        private string _fxPrefab;
        
        private float _scale;
        private readonly Collider2D[] _results = new Collider2D[32];
        
        public void Init(CurrentSkillData skillData)
        {
            _damage = skillData.SkillAbility.SkillDamage;
            _effectDamage = skillData.SkillAbility.EffectDamage;
            _duration =  skillData.SkillAbility.EffectDuration;
            _tickInterval = skillData.SkillAbility.DotDamageInterval;
            _fxPrefab = skillData.SkillRow.Fx;
        }
        
        public void OnHit(Vector2 hitPosition, BaseActor hitTarget)
        {
            hitTarget.TakeDamage(_damage); // 타격 데미지
            var fx = PlayFx(hitPosition);
            
            if (fx is not null)
            {
                Vector2 size = Define.ProjectileFxCapsuleSize * fx.transform.localScale.x * (1 + _scale / 1000f);
                ApplyPoisonDamage(hitPosition,size); // 스플래시 데미지
            }
        }
        
        private void ApplyPoisonDamage(Vector2 position, Vector2 size)
        {
            int count = Physics2D.OverlapCapsuleNonAlloc(
                position,
                size,
                CapsuleDirection2D.Horizontal,
                0f,
                _results,
                _targetLayer
            );

            for (int i = 0; i < count; i++)
            {
                var col = _results[i];

                if (col is null)
                    continue;

                if (!col.TryGetComponent<MonsterActor>(out var actor))
                    continue;
                
                actor.OnMonsterDotDamage(_effectDamage, _duration, _tickInterval);
            }
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