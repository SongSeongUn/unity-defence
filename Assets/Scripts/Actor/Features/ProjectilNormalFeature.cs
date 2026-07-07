using UnityEngine;
using Interface;
using Battle.Skills;

namespace Actor.Features
{
    public class ProjectilNormalFeature : MonoBehaviour, IProjectileHitFeature
    {
        [Header("타겟 레이어")]
        [SerializeField] private LayerMask _targetLayer;
        
        private int _damage;
        private string _fxPrefab;
        
        public void Init(CurrentSkillData skillData)
        {
            _damage = skillData.SkillAbility.SkillDamage;
            _fxPrefab = skillData.SkillRow.Fx;
        }
        
        public void OnHit(Vector2 hitPosition, BaseActor hitTarget)
        {
            hitTarget.TakeDamage(_damage); // 타격 데미지
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