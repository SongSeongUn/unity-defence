using Cysharp.Threading.Tasks;
using Events;
using Managers;
using System.Collections.Generic;
using Events.Player;
using UI.UIItem;
using UnityEngine;

namespace UI.WorldCanvas
{
    public class UI_MonsterStatus_World : UIWorldCanvas
    {
        private const string MonsterHpBarPoolTag = "UIMonsterHpBar";
        private readonly Dictionary<MonsterActor, UIMonsterHpBar> _hpBars = new();
        private bool _isPoolReady;

        protected override void Awake()
        {
            base.Awake();
            InitAsync().Forget();
        }

        private void OnEnable()
        {
            GameEvents.AddListener<MonsterDamagedEvent>(OnMonsterDamaged);
            GameEvents.AddListener<ActorDiedEvent>(OnActorDied);
        }

        private void OnDisable()
        {
            GameEvents.RemoveListener<MonsterDamagedEvent>(OnMonsterDamaged);
            GameEvents.RemoveListener<ActorDiedEvent>(OnActorDied);
        }
        
        private void Start()
        {
            GameEvents.AddListener<PlayerLevelUpEvent>(OnPlayerLevelUp);
            GameEvents.AddListener<PlayerSkillSelecdtEvent>(OnPlayerSkillSelect);
        }

        private void OnDestroy()
        {
            GameEvents.RemoveListener<PlayerLevelUpEvent>(OnPlayerLevelUp);
            GameEvents.RemoveListener<PlayerSkillSelecdtEvent>(OnPlayerSkillSelect);
            ClearHpBars();
        }

        private async UniTaskVoid InitAsync()
        {
            await ObjectPoolManager.Instance.InitObjectPool<UIMonsterHpBar>("UI/UIMonsterHpBar.prefab");
            _isPoolReady = true;
        }

        public void Show()
        {
            Open();
        }

        private void OnPlayerLevelUp(PlayerLevelUpEvent evt)
        {
            Close();
        }

        private void OnPlayerSkillSelect(PlayerSkillSelecdtEvent evt)
        {
            Open();
        }

        private void OnMonsterDamaged(MonsterDamagedEvent evt)
        {
            if (evt?.Actor is not MonsterActor monster)
                return;

            ShowOrRefreshHpBar(monster);
        }

        private void OnActorDied(ActorDiedEvent evt)
        {
            if (evt?.Actor is not MonsterActor monster)
                return;

            HideHpBar(monster);
        }

        private void ShowOrRefreshHpBar(MonsterActor actor)
        {
            if (!_isPoolReady || actor == null)
                return;

            if (_hpBars.TryGetValue(actor, out var hpBar))
            {
                hpBar.Refresh();
                return;
            }

            hpBar = ObjectPoolManager.Instance.Get<UIMonsterHpBar>(MonsterHpBarPoolTag, Vector3.zero);
            if (hpBar == null)
                return;

            hpBar.Init(actor, UIManager.Instance.MainCamera, transform);
            _hpBars.Add(actor, hpBar);
        }

        private void HideHpBar(MonsterActor actor)
        {
            if (actor == null || !_hpBars.TryGetValue(actor, out var hpBar))
                return;

            hpBar.ReturnToPool();
            _hpBars.Remove(actor);
        }

        private void ClearHpBars()
        {
            foreach (var hpBar in _hpBars.Values)
            {
                hpBar.ReturnToPool();
            }

            _hpBars.Clear();
        }
    }
}