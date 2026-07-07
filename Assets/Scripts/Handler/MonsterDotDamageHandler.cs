using System;
using Battle.Skills;
using Unity.Mathematics;
using UnityEngine;

namespace Handler
{
    public class MonsterDotDamageHandler : MonoBehaviour
    {
        [SerializeField] private MonsterActor _monsterActor;
        
        private int _damage;
        private float _tickInterval;

        private float _remainTime;
        private float _tickTimer;
        
        private bool _isActive = false;

        public void Init(int damage, int duration, int tickInterval)
        {
            _damage = damage;
            _remainTime = Math.Max(_remainTime, duration / 1000f);
            _tickInterval = tickInterval / 1000f;
            _tickTimer = _tickInterval; // 바로 데미지 들어가지 않게
            _isActive = true;
        }
        
        private void Update()
        {
            if (!_isActive)
                return;

            _remainTime -= Time.deltaTime;
            _tickTimer -= Time.deltaTime;

            if (_tickTimer <= 0f)
            {
                _monsterActor.TakeDamage(_damage);
                //DebugUtils.Log(this.name + " poison damaged " + _damage);
                _tickTimer = _tickInterval;
            }

            if (_remainTime <= 0f)
            {
                _isActive = false;
            }
        }

        private void OnDisable()
        {
            _isActive = false;
            _remainTime = 0f;
            _tickTimer = 0f;
        }
    }
}