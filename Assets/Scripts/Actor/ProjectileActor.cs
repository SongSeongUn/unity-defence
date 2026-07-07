using System;
using System.Collections;
using Battle.Skills;
using UnityEngine;
using Handler;


public class ProjectileActor : MonoBehaviour
{
    [SerializeField] private ProjectileMoveHandler _projectileMoveHandler;
    [SerializeField] private ProjectileHitHandler _projectileHitHandler;

    private bool _isReturned = false;
    private Coroutine _lifeCoroutine;
    // 타겟 및 상태 변수
    private string _Prefab;
    
    /// <summary>
    /// 투사체 발사 초기화 함수
    /// </summary>
    public void Init(CurrentSkillData skillData, Transform target, Transform firePoint)
    {
        _isReturned = false;
        
        if (_lifeCoroutine != null)
            StopCoroutine(_lifeCoroutine);
        
        _Prefab = skillData.SkillRow.Prefab;
        
        _projectileMoveHandler.Init(skillData, target, firePoint);
        _projectileHitHandler.Init(this, skillData);
        
        _lifeCoroutine = StartCoroutine(CoDestroyAfterTime(3f));
    }
    

    private IEnumerator CoDestroyAfterTime(float lifeTime)
    {
        yield return new WaitForSeconds(lifeTime);
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (_isReturned)
            return;

        _isReturned = true;

        if (_lifeCoroutine != null)
        {
            StopCoroutine(_lifeCoroutine);
            _lifeCoroutine = null;
        }
        
        ObjectPoolManager.Instance.Return(_Prefab, this);
        gameObject.SetActive(false);
    }
}
