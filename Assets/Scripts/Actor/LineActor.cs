using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Battle.Skills;

namespace Actor
{
    public class LineActor : MonoBehaviour
    {
        [SerializeField] protected LineRenderer _lineRenderer;
        // [SerializeField] private Animator _animator; // 애니메이션 등록할거면
        protected float _lifeTime; // Fx 라이프타임 
        protected CurrentSkillData _skillData;
        
        
        public virtual void Init(CurrentSkillData skillData, Vector2 startPos, Vector2 endPos, float lifeTime = 0.6f)
        {
            _skillData = skillData;
            _lifeTime = lifeTime;
        }

        protected virtual void SetLine(Vector2 startPos, Vector2 endPos)
        {
            // 시작 위치
            _lineRenderer.SetPosition(0, startPos);
            _lineRenderer.SetPosition(1, endPos);
        }
    }
}