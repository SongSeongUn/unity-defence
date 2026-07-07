using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Handler
{
    public class MonsterAttackHandler : MonoBehaviour
    {
        private MonsterActor _monsterActor;
        private WallActor _wallActor;
        private int _damage = 0;
        private float _nextAttackTime;
        
        private System.Threading.CancellationTokenSource _attackCts;

        private void OnDisable()
        {
            OnStopAttack();
        }

        public void Init(MonsterActor actor, WallActor wallActor)
        {
            _monsterActor = actor;
            _wallActor = wallActor;
            _damage = actor.MonsterConfig.Atk;
        }

        public void OnStartAttack()
        {
            OnStopAttack();
            _attackCts = new System.Threading.CancellationTokenSource();
            AttackLoopAsync().Forget();
        }

        public void OnStopAttack()
        {
            _attackCts?.Cancel();
            _attackCts?.Dispose();
            _attackCts = null;
        }
        
        private async UniTaskVoid AttackLoopAsync()
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f),
                    cancellationToken: _attackCts.Token
                );
            
                while (!_attackCts.Token.IsCancellationRequested && _wallActor.IsAlive)
                {
                    if (_attackCts.Token.IsCancellationRequested) break;
                
                    await UniTask.Delay(TimeSpan.FromSeconds(_monsterActor.MonsterConfig.AtkSpeed / 1000f),
                        cancellationToken: _attackCts.Token);
                
                    _wallActor.TakeDamage(_damage);
                }
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
    }
}