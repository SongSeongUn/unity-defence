using System.Collections;
using UnityEngine;

public class FxActor : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 0.6f;
    [SerializeField] private Animator _animator;

    private Vector3 defaultScale;
    private string _prefab;
    
    private bool _isReturned = false;
    private Coroutine _lifeCoroutine;

    private void Awake()
    {
        if (_animator == null)
            TryGetComponent(out _animator);

        defaultScale = transform.localScale;
    }

    public void Init(string prefab, float scale = 1, float lifeTime = 0.6f)
    {
        _prefab = prefab;
        _lifeTime = lifeTime;
        transform.localScale = defaultScale * scale;
        
        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
        }
        
        _isReturned = false;
        
        if (_lifeCoroutine != null)
            StopCoroutine(_lifeCoroutine);
        
        _lifeCoroutine = StartCoroutine(ReturnAfterLifeTime());
    }

    private IEnumerator ReturnAfterLifeTime()
    {
        yield return new WaitForSeconds(_lifeTime);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (_isReturned)
            return;

        _isReturned = true;

        if (_lifeCoroutine != null)
        {
            StopCoroutine(_lifeCoroutine);
            _lifeCoroutine = null;
        }

        if (!string.IsNullOrWhiteSpace(_prefab) && ObjectPoolManager.Instance != null)
            ObjectPoolManager.Instance.Return(_prefab, this);
        else
            gameObject.SetActive(false);
    }
}
