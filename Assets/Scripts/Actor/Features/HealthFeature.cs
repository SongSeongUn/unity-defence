using Common;
using Events;
using UnityEngine;

namespace Actor.Features
{
    public class HealthFeature : MonoBehaviour
    {
        private long _hp;
        private long _maxHp;
        private BaseActor _actor;
        private bool _isDead;

        public long Hp => _hp;
        public long MaxHp => _maxHp;
        public bool IsDead => _isDead;

        public void Init(long hp, BaseActor actor)
        {
            _hp = hp;
            _maxHp = hp;
            _actor = actor;
            _isDead = false;
        }

        public void TakeDamage(int damage)
        {
            if (_isDead)
                return;

            _hp -= damage;

            if (_actor is MonsterActor)
                GameEvents.SendEvent(new MonsterDamagedEvent(_actor));

            if (_hp <= 0)
            {
                _isDead = true;
                GameEvents.SendEvent(new ActorDiedEvent(_actor));
            }
        }
    }
}
