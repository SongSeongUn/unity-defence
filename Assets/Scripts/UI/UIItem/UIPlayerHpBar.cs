using System;
using Events;
using Events.Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIItem
{
    public class UIPlayerHpBar : MonoBehaviour
    {
        [SerializeField] private Slider _hpSlider;

        private void OnEnable()
        {
            GameEvents.AddListener<PlayerDamagedEvent>(OnPlayerDamaged);
        }

        private void OnDisable()
        {
            GameEvents.RemoveListener<PlayerDamagedEvent>(OnPlayerDamaged);
        }

        private void Awake()
        {
            _hpSlider.value = 1;
        }

        private void OnPlayerDamaged(PlayerDamagedEvent evt)
        {
            float hpRate = evt.Actor.MaxHp <= 0 ? 0f : Mathf.Clamp01((float)evt.Actor.Hp / evt.Actor.MaxHp);
            _hpSlider.value = hpRate;
        }
    }
}