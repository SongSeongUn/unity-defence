using UnityEngine;
using UnityEngine.UI;

namespace UI.UIItem
{
    public class UIMonsterHpBar : MonoBehaviour
    {
        private const string PoolTag = "UIMonsterHpBar";

        [SerializeField] private Slider _hpSlider;
        [SerializeField] private Vector3 _worldOffset = new(0f, -0.35f, 0f);

        private RectTransform _rectTransform;
        private MonsterActor _target;
        private Camera _worldCamera;

        private void Awake()
        {
            TryGetComponent(out _rectTransform);
        }

        public void Init(MonsterActor actor, Camera worldCamera, Transform parent)
        {
            _target = actor;
            _worldCamera = worldCamera;
            transform.SetParent(parent, false);
            Refresh();
            FollowTarget();
        }

        public void Refresh()
        {
            if (_hpSlider == null || _target == null)
                return;

            float hpRate = _target.MaxHp <= 0 ? 0f : Mathf.Clamp01((float)_target.Hp / _target.MaxHp);
            _hpSlider.value = hpRate;
        }

        private void LateUpdate()
        {
            if (_target == null || _target.IsDead || !_target.gameObject.activeInHierarchy)
                return;

            FollowTarget();
        }

        private void FollowTarget()
        {
            if (_target == null)
                return;

            Vector3 worldPosition = _target.transform.position + _worldOffset;
            Vector3 screenPosition = _worldCamera != null
                ? _worldCamera.WorldToScreenPoint(worldPosition)
                : worldPosition;

            if (_rectTransform != null)
                _rectTransform.position = screenPosition;
            else
                transform.position = screenPosition;
        }

        public void ReturnToPool()
        {
            _target = null;
            _worldCamera = null;

            if (_hpSlider != null)
                _hpSlider.value = 0f;

            if (ObjectPoolManager.Instance != null)
                ObjectPoolManager.Instance.Return(PoolTag, this);
            else
                gameObject.SetActive(false);
        }
    }
}
