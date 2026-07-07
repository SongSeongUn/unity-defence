using Handler;
using UnityEngine;
using Battle;
using Events;

public enum MonsterGrade
{
    Normal = 1,
    MiddleBoss,
    Boss,
}


public class MonsterActor : BaseActor 
{
    [SerializeField] private MonsterAttackHandler _attackHandle;
    [SerializeField] private MonsterMoveHandler _moveHandle;
    [SerializeField] private MonsterDotDamageHandler _dotDamageHandler;
    
    public MonsterConfig MonsterConfig { get; private set; }
    public MonsterGrade Grade => (MonsterGrade)MonsterConfig.Grade;

    public bool IsInMap => _moveHandle.IsInMap;

    public void Init(MonsterConfig config, WallActor wallActor)
    {
        MonsterConfig = config;
        base.Init(config.Hp);
        
        _moveHandle.Init(this, wallActor);
        _attackHandle.Init(this, wallActor);
    }

    public void OnReachedDestination()
    {
        _attackHandle.OnStartAttack();
    }

    public void OnMonsterSlow(int slowRate, int duration)
    {
        _moveHandle.OnApplySlow(slowRate, duration);
    }

    public void OnMonsterDotDamage(int damage, int duration, int interval)
    {
        _dotDamageHandler.Init(damage, duration, interval);
    }

    // protected override void OnDamaged()
    // {
    //     GameEvents.SendEvent(new MonsterDamagedEvent(this));
    // }
}