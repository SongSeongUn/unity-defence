using Battle.Skills;
using Interface;
using UnityEngine;

namespace Actor.Features
{
    public class ProjectileExplosionFeature : MonoBehaviour, IProjectileHitFeature
    {
        [Header("타겟 레이어")]
        [SerializeField] private LayerMask _targetLayer;
        
        private int _damage;
        private int _effectDamage;
        private string _fxPrefab;

        private float _scale;
        private readonly Collider2D[] _results = new Collider2D[32];
        
#if UNITY_EDITOR
        // 확인용
        private Vector2 _lastExplosionPos;
#endif
        
        public void Init(CurrentSkillData skillData)
        {
            _damage = skillData.SkillAbility.SkillDamage;
            _effectDamage = skillData.SkillAbility.EffectDamage;
            _fxPrefab = skillData.SkillRow.Fx;
            _scale = skillData.SkillAbility.Scale;
        }
        
        public void OnHit(Vector2 hitPosition, BaseActor hitTarget)
        {
            hitTarget.TakeDamage(_damage); // 타격 데미지
            
            var fx = PlayFx(hitPosition);
            if (fx is not null)
            {
                float radius = Define.ProjectileFxBaseRadius * fx.transform.localScale.x * (1 + _scale / 1000f);
                ApplyExplosionDamage(hitPosition, hitTarget, radius); // 스플래시 데미지
                
#if UNITY_EDITOR
                Common.BattleDebugDraw.DrawExplosionRadius(hitPosition, radius);
#endif
            }
        }
        
        private void ApplyExplosionDamage(Vector2 position, BaseActor directHitTarget, float radius)
        {
            int count = Physics2D.OverlapCircleNonAlloc(position, radius, _results, _targetLayer);
            //DebugUtils.Log($"폭발 대상 수 {count}");

            for (int i = 0; i < count; i++)
            {
                var col = _results[i];

                if (col is null)
                    continue;

                if (!col.TryGetComponent<BaseActor>(out var actor))
                    continue;

                if (actor == directHitTarget) // 타격 대상이면 제외
                    continue;
                
                actor.TakeDamage(_effectDamage);
            }
        }

        private FxActor PlayFx(Vector2 position)
        {
            if (string.IsNullOrWhiteSpace(_fxPrefab) || ObjectPoolManager.Instance is null)
                return null;

            FxActor fx = ObjectPoolManager.Instance.Get<FxActor>(_fxPrefab, position);
            fx?.Init(_fxPrefab, _scale);
            return fx;
        }
    }
}