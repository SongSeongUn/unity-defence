using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Events;
using Handler;
using Managers;
using Table.Models;
using Table.TBL;
using UI.Popup;
using UI.UIWindow;
using UI.WorldCanvas;
using static Managers.DataTableManager;


public class StageController : MonoBehaviour
{
    [SerializeField] private PlayerLevelUpHandler _playerLevelUpHandler;
    [SerializeField] private StageWaveHandler _stageWaveHandler;
    [SerializeField] private WaveSpawnHandler _waveSpawnHandler;
    [SerializeField] private MonsterSpawnController _monsterSpawnController;
    
    private int _currentStage = 1;
    
    private UI_MainStage_Page _stageUI;
    
    [Header("누적 겸험치")]
    public int TotalExp { get; set; } = 0;
    
    
    private void OnEnable()
    {
        GameEvents.AddListener<Events.ActorDiedEvent>(OnActorDied);
    }

    private void OnDisable()
    {
        GameEvents.RemoveListener<Events.ActorDiedEvent>(OnActorDied);
    }

    private void Start()
    {
        StartAsync().Forget();
    }
    
    private async UniTask StartAsync()
    {
        // TODO 데이터 관리
        _currentStage = 1;

        try
        {
            await UIManager.Create();
            var stageUI = await UIManager.Instance.OpenUI<UI_MainStage_Page>("UI_MainStage_Page");
            _stageUI = stageUI;
            stageUI.Show(new UIStageData{ Stage = _currentStage, Wave = 0 });
            
            var statusUI = await UIManager.Instance.OpenUI<UI_MonsterStatus_World>("UI_MonsterStatus_World");
            statusUI.Show();

            _stageWaveHandler.Init(_currentStage, (waveData) =>
            {
                RefreshExpUI(0, 1);
                _waveSpawnHandler.Init(waveData);
            });
        }
        catch (Exception e)
        {
            DebugUtils.LogError(e.Message);
            throw;
        }
    }

    public void RefreshExpUI(int exp, int currentLevel)
    {
        BattleExpRow expRow = GetTable<BattleExpTable>()?.GetData(currentLevel);
        if (expRow == null)
            return;

        _stageUI?.RefreshExp(exp, expRow.Need_Exp);
    }
    

    private void OnActorDied(Events.ActorDiedEvent evt)
    {
        if (evt is null)
        {
            DebugUtils.LogError("evt is Null");
            return;
        }

        if (evt.Actor is MonsterActor mon)
        {
            // 몬스터 사망
            _monsterSpawnController.ReturnMonster(mon);
            _playerLevelUpHandler.AddExp(mon);
        }
        else
        {
            // 플레이어 사망
            Time.timeScale = 0;
            UIManager.Instance.OpenUI<UI_Fail_Popup>("UI_Fail_Popup").Forget();
        }
    }
}
