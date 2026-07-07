using System;
using UnityEngine;
using Interface;
using System.Collections.Generic;
using Battle.Skills;
using Random = UnityEngine.Random;

namespace Handler
{
    public class ProjectileHitHandler : MonoBehaviour
    {
        [Header("스플래시 설정")] 
        public LayerMask monsterLayer; // 몬스터 레이어
        private bool _isPearce = true;

        private IProjectileHitFeature[] _hitFeatures;
        
        private Collider2D[] _hitResults = new Collider2D[20];
        private HashSet<BaseActor> _hitActors = new();

        private ProjectileActor _actor;
        
        private void Awake()
        {
            _hitFeatures = GetComponents<IProjectileHitFeature>();
        }
        
        public void Init(ProjectileActor actor, CurrentSkillData skillData)
        {
            _actor = actor;
            _hitActors.Clear();
            
            // 스킬 정보 hitFeature 세팅
            if (_hitFeatures != null || _hitFeatures.Length == 0)
                foreach (var feature in _hitFeatures)
                    feature.Init(skillData);
        
            // 관통 여부
            _isPearce = skillData.SkillAbility.PierceRate > 0 &&  Random.Range(0, 100000) < skillData.SkillAbility.PierceRate;
        }

        private void Update()
        {
            int count = Physics2D.OverlapCircleNonAlloc(transform.position,  0.05f,  _hitResults, monsterLayer);
            if (count > 0)
            {
                float minDistance = float.MaxValue;
                BaseActor nearest = null;

                for (int i = 0; i < count; i++)
                {
                    Collider2D hitTarget = _hitResults[i];

                    if (hitTarget is null)
                        continue;
                
                    if (!hitTarget.TryGetComponent<BaseActor>(out var actor))
                        continue;
                
                    if (_hitActors.Contains(actor))
                        continue;

                    float dist = (hitTarget.transform.position - transform.position).sqrMagnitude;

                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nearest = actor;
                    }
                }

                if (nearest is not null)
                {
                    foreach (var feature in _hitFeatures)
                        feature.OnHit((Vector2)transform.position, nearest);
                    _hitActors.Add(nearest);
                } 
                // 투사체 회수
                if (!_isPearce) _actor.ReturnToPool();
            }
        }
    }
}