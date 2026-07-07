using Actor.Features;
using UnityEngine;


public class BaseActor : MonoBehaviour
{
    [SerializeField] private HealthFeature _health;

    public long Hp => _health != null ? _health.Hp : 0;
    public long MaxHp => _health != null ? _health.MaxHp : 0;
    public bool IsDead => _health != null && _health.IsDead;

    public virtual void Init(long hp)
    {
        _health.Init(hp, this);
    }

    public void TakeDamage(int damage)
    {
        // 보호막이 생기면 추가
        _health.TakeDamage(damage);
        OnDamaged();
    }

    protected virtual void OnDamaged()
    {

    }
}
