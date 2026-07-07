using Battle.Skills;
using UnityEngine;

namespace Actor.Features
{
    public class ChainLightningFeature : MonoBehaviour
    {
        private int _damage;
        private string _fxPrefab;
        
        public void Init(CurrentSkillData skillData)
        {
            _damage = skillData.SkillAbility.EffectDamage;
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