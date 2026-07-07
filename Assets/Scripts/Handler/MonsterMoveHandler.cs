using System;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Handler
{
    public class MonsterMoveHandler : MonoBehaviour
    {
        private MonsterActor _monsterActor;
        private float randomPosYOffset; // 멈추는 좌표
        private int _moveSpeed = 0;
        
        public bool IsInMap { get; private set; } = false;
        public bool isReached = false;

        private CancellationTokenSource _slowCts;
        public void Init(MonsterActor actor, WallActor wallActor)
        {
            IsInMap = false;
            isReached = false;
            _monsterActor = actor;
            randomPosYOffset = wallActor.WallTop + UnityEngine.Random.Range(0f, 0.2f); // 멈출 위치에서 약간 떨어진 거리
            _moveSpeed = actor.MonsterConfig.MoveSpeed;
        }

        public void OnApplySlow(int value, int duration)
        {
            _moveSpeed = _monsterActor.MonsterConfig.MoveSpeed 
                         - Mathf.Min(_monsterActor.MonsterConfig.MoveSpeed, 
                             _monsterActor.MonsterConfig.MoveSpeed * value / 100000);
            
            _slowCts?.Cancel();
            _slowCts?.Dispose();
            _slowCts = new CancellationTokenSource();
            
            RemoveSlowAfterAsync(duration).Forget();
        }

        private async UniTaskVoid RemoveSlowAfterAsync(int duration)
        {
            try
            {
                TimeSpan delay = TimeSpan.FromSeconds(duration / 1000f);
                if (!_slowCts.Token.IsCancellationRequested)
                    await UniTask.Delay(delay, cancellationToken : _slowCts.Token);
                
                _moveSpeed = _monsterActor.MonsterConfig.MoveSpeed;
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception e)
            {
                DebugUtils.LogError(e);
                throw;
            }
        }
        
        private void Update()
        {
            if (!IsInMap)
            {
                if (transform.position.y <= 4.5f) // 시야에 보이는 시점
                {
                    IsInMap = true;
                }
            }

            if (isReached) return;

            //transform.Translate(_speed * 0.1f * Time.deltaTime * Vector3.down);
            transform.Translate((float)_moveSpeed / 1000 / Define.BgDistance * Time.deltaTime * Vector3.down, Space.World);
            if (transform.position.y <= randomPosYOffset)
            {
                isReached = true;
                _monsterActor.OnReachedDestination();
            }
        }
    }
}
