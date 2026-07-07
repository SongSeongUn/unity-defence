using System;
using UnityEngine;
using System.Collections.Generic;
using Interface;
using Core;
using Actor.Features;

namespace Battle.Skills
{
    public class ChainSkillExecutor : SkillExecutor
    {
        [SerializeField] private MonoBehaviour targetProviderObject;
        private IMonsterTargetProvider targetProvider;

        private void Awake()
        {
            targetProvider = targetProviderObject as IMonsterTargetProvider;
            
            if (targetProvider == null)
                DebugUtils.LogError("TargetProvider is Null");
        }

        public override void ExecuteSkill(CurrentSkillData skillData, Transform target, Transform firePoint)
        {
            // firePoint 부터 첫번째 타겟
            // DebugUtils.Log($"공격 대상 : {target.name}");
            InitLine(skillData, firePoint.position, target.transform.position, target);

            // 타겟부터 체인 타겟
            var chainTarget = targetProvider.FindNearestChain(target.transform.position,
                skillData.SkillAbility.EffectCount);

            // for (int i = 0; i < chainTarget.Count; i++)
            //     DebugUtils.Log($"튕기는 대상 {i + 1} : {chainTarget[i].name}");

            for (int i = 0; i < chainTarget.Count; i++)
            {
                Transform startTr = i == 0 ? target.transform : chainTarget[i - 1].transform;
                Transform endTr = chainTarget[i].transform;

                InitLine(skillData, startTr.position, endTr.position, chainTarget[i].transform);
            }
        }

        private void InitLine(CurrentSkillData skillData, Vector3 startPos, Vector3 endPos, Transform target)
        {
            var targetPoints = DynamicAimLinear.GetSplitPoints(startPos, endPos, 1);
            for (int i = 0; i < targetPoints.Count; i++)
            {
                var lineActor = ObjectPoolManager.Instance.Get<ChainLightningActor>(skillData.SkillRow.Prefab, 
                    startPos);

                if (lineActor == null)
                {
                    DebugUtils.LogError("ChainLightningActor Not Found");
                    break;
                }
                
                if (i == 0) 
                    lineActor.Init(skillData, startPos, targetPoints[i]);
                else
                    lineActor.Init(skillData, targetPoints[i - 1], targetPoints[i]);

                if (i == targetPoints.Count - 1 && target is not null)
                {
                    if (!target.TryGetComponent(out BaseActor actor))
                    {
                        DebugUtils.LogError("Target BaseActor Not Found");
                        return;
                    }
                    
                    lineActor.OnHit(skillData, targetPoints[i], actor);
                }
            }
        }
    }
}