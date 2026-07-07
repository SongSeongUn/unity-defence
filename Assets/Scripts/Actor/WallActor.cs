using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Events;
using Events.Player;

public class WallActor : BaseActor 
{
    [SerializeField] private BoxCollider2D _wallCollider;

    [Header("피격 설정")]
    [SerializeField] private SpriteRenderer _wallSprite;
    [SerializeField] private Color hitColor;

    [SerializeField] private PlayerConfigSO config;

    [Tooltip("색상이 변경되었다가 돌아오는 시간 (초)")]
    public float flashDuration = 0.1f;
    public bool IsAlive { get; private set; } = true;
    
    public float WallTop => _wallCollider.bounds.max.y;
    
    Color originColor;
    int _flashVersion;
    
    private CancellationTokenSource _hitCts;

    private void Awake()
    {
        originColor = _wallSprite.color;
    }

    private void Start()
    {
        base.Init(config.WallHP);
        _hitCts =  new CancellationTokenSource();
    }

    protected override void OnDamaged()
    {
        HitFlash().Forget();
        GameEvents.SendEvent(new PlayerDamagedEvent(this));
    }

    private async UniTaskVoid HitFlash()
    {
        try
        {
            if (_wallSprite == null)
                return;

            int version = ++_flashVersion;

            _wallSprite.color = hitColor;

            await UniTask.Delay(TimeSpan.FromSeconds(flashDuration), cancellationToken: _hitCts.Token);

            if (version != _flashVersion)
                return;

            _wallSprite.color = originColor;
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception e)
        {
            DebugUtils.LogError(e);
            throw;
        }
    }
    
    private void OnDestroy()
    {
        if (_hitCts != null)
        {
            _hitCts.Cancel();
            _hitCts.Dispose();
        }
    }
}