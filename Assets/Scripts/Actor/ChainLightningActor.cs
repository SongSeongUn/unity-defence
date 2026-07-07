using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Battle.Skills;

namespace Actor.Features
{
    public class ChainLightningActor : LineActor
    {
        private const float MIN_LENGTH = 0.1f;
        private const float MIN_SCALE = 0.1f;
        private string _Prefab;

        private bool _isTargetLine = false;
        
        private ChainLightningFeature _features;

        public BaseActor Target;
        
        private bool _isReturned = false;
        private Coroutine _lifeCoroutine;

        private void Awake()
        {
            _features = GetComponent<ChainLightningFeature>();
            if (_features == null)
                DebugUtils.LogError("ChainLightningFeature is Null");
        }
        
        public void Init(CurrentSkillData skillData, Vector2 startPos, Vector2 endPos, float lifeTime = 0.4f)
        {
            base.Init(skillData, startPos, endPos, lifeTime);
            _Prefab = skillData.SkillRow.Prefab;
            _lifeTime = lifeTime;
            SetLine(startPos, endPos);
            
            _isReturned = false;
        
            if (_lifeCoroutine != null)
                StopCoroutine(_lifeCoroutine);
            
            _lifeCoroutine = StartCoroutine(ReturnAfterLifeTime());
        }

        
        
        protected override void SetLine(Vector2 startPos, Vector2 endPos)
        {
            // 시작 위치
            base.SetLine(startPos, endPos);
            
            float distance = Vector2.Distance(startPos, endPos);
            _lineRenderer.textureScale = new Vector2(distance, 1); // LineRenderer 최소 스케일
        }
        
        public void OnHit(CurrentSkillData skillData, Vector2 hitPos, BaseActor hitTarget)
        {
            _features.Init(skillData);
            _features.OnHit(hitPos, hitTarget);
        }
        
        
        private IEnumerator ReturnAfterLifeTime()
        {
            yield return new WaitForSeconds(_lifeTime);
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (_isReturned)
                return;

            _isReturned = true;

            if (_lifeCoroutine != null)
            {
                StopCoroutine(_lifeCoroutine);
                _lifeCoroutine = null;
            }

            if (!string.IsNullOrWhiteSpace(_Prefab) && ObjectPoolManager.Instance != null)
                ObjectPoolManager.Instance.Return(_Prefab, this);
            else
                gameObject.SetActive(false);
        }
    }
}