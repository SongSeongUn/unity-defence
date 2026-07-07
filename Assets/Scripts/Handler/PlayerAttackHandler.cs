using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Battle.Skills;
using Events;
using Events.Player;
using Handler;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAttackHandler : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("플레이어로부터 포인트 떨어져 있는 거리 (파란색 부채꼴의 반지름)")]
    public float OrbitRadius = 0.5f;
    
    [SerializeField] private MonoBehaviour targetProviderObject;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private BoxCollider2D _wallCollider;
    
    [SerializeField] private SkillExecutionHandler _skillExecutionHandler;
    [FormerlySerializedAs("_skillInventory")] [SerializeField] private SkillInventoryController skillInventoryController;
    
    [SerializeField] private Animator _animator;
    
    private bool _isAttacking = false;
    private float _nextGlobalAttackTime = 0f;
    [SerializeField] private float globalCooldown = 0.5f; // 스킬 글로벌 딜레이
    
    private MonsterActor _monsterTarget;

    private System.Threading.CancellationTokenSource spawnCts;
    private Interface.IMonsterTargetProvider targetProvider;
    
    private void OnEnable()
    {
        GameEvents.AddListener<ActorDiedEvent>(OnActorDied);
    }

    private void OnDisable()
    {
        GameEvents.RemoveListener<ActorDiedEvent>(OnActorDied);
    }

    private void Awake()
    {
        //_defaultFirePointTr = _firePoint.transform;
        targetProvider = targetProviderObject as Interface.IMonsterTargetProvider;
    }

    private void Start()
    {
        StartSetupAsync().Forget();
    }
    
    private void OnActorDied(ActorDiedEvent evt)
    {
        if (evt.Actor is MonsterActor)
            InitLoopAsync();
    }

    /// <summary>
    /// 루프 초기화
    /// </summary>
    public void InitLoopAsync()
    {
        _monsterTarget = null;
        spawnCts?.Cancel();
        spawnCts?.Dispose();

        spawnCts = new System.Threading.CancellationTokenSource();
        //AimAt(_defaultFirePointTr.position);
        AttackLoopAsync().Forget();
    }

    // 💡 초기화와 루프 실행을 순서대로 완벽하게 보장하는 비동기 묶음 함수
    private async UniTaskVoid StartSetupAsync()
    {
        await skillInventoryController.InitDefaultProjectileAsync();

        spawnCts = new System.Threading.CancellationTokenSource();
        AttackLoopAsync().Forget();
    }

    private async UniTaskVoid AttackLoopAsync()
    {
        while (this != null && gameObject.activeInHierarchy)
        {
            // 게임 오버나 중단 요청 시 루프 탈출
            if (spawnCts.Token.IsCancellationRequested)
                return;

            if (_monsterTarget == null || !_monsterTarget.gameObject.activeInHierarchy)
            {
                var target = targetProvider.FindNearest(_wallCollider.bounds.max.y); // WallPosTop
                if (target is null)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: spawnCts.Token);
                    continue;
                }
                _monsterTarget = target;
            }
            
            TryFireReadySkill(_monsterTarget.transform);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: spawnCts.Token);
        }
    }
    
    
    private void TryFireReadySkill(Transform target)
    {
        float currentTime = Time.time;
        var readySkills = skillInventoryController.CurrentSkills
            .Where(x => currentTime >= x.NextSpawnTime)
            .OrderBy(x => x.NextSpawnTime)
            .ToList();
        
        if (readySkills.Count > 0 && currentTime >= _nextGlobalAttackTime)
        {
            var readySkill = readySkills[0];
            var cooldown = readySkill.SkillAbility.Cooldown / 1000;
            // 상태 업데이트
            readySkill.NextSpawnTime = currentTime + cooldown;
            _nextGlobalAttackTime = currentTime + globalCooldown;
            
            _animator.SetTrigger("Attack");
            GameEvents.SendEvent(new PlayerSkillCoolDownEvent(
                readySkill.SkillRow.No,
                readySkill.NextSpawnTime,
                cooldown));
            
            
            _skillExecutionHandler.ExecuteSkill(readySkill, target.transform, _firePoint);
        }
    }
}
