using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Events;
using Battle;

namespace Handler
{
    public class WaveSpawnHandler : MonoBehaviour
    {
        [SerializeField] private MonsterSpawnController _monsterSpawnController;
        private StageController _stageController;
        
        private List<MonsterSpawnWaveData> _waveDatas = new ();
        
        private System.Threading.CancellationTokenSource spawnCts;
        
        // 상태
        private float _nextWaveTime = 0;
        
        public void Init(List<MonsterSpawnWaveData> waveDatas)
        {
            _waveDatas =  waveDatas;

            StopSpawning();
            StartSpawning().Forget();
        }
    
        private async UniTaskVoid StartSpawning()
        {
            int currentWave = 1;
            int lastWave = _waveDatas.Count;
            spawnCts = new System.Threading.CancellationTokenSource();
            
            SendWaveStartEvent(currentWave, _waveDatas[0].WaveStartInterval);

            while (true)
            {
                // 게임 오버나 중단 요청 시 루프 탈출
                if (spawnCts.Token.IsCancellationRequested) break;

                float currentTime = Time.time;
                var waveData = _waveDatas[currentWave - 1];

                if (currentWave == lastWave &&
                    _waveDatas[lastWave - 1].SpawnedCount >= _waveDatas[lastWave - 1].MaxCount)
                {
                    DebugUtils.Log("몬스터 소환 완료 종료 대기");
                    break;
                }

                if (currentTime >= _nextWaveTime)
                {
                    currentWave++;
                    SendWaveStartEvent(currentWave, waveData.WaveStartInterval);

                    // TODO : 웨이브 시작 로고 나올 수 있음 여기에

                    // 웨이브 변경으로 이번 프레임 스킵
                    await UniTask.Yield(PlayerLoopTiming.Update, spawnCts.Token);
                    continue;
                }

                if (currentTime >= waveData.NextSpawnTime)
                {
                    int remainingToSpawn = waveData.MaxCount - waveData.SpawnedCount;
                    int spawnTargetCount = Mathf.Min(waveData.MaxSpawnCount, remainingToSpawn);

                    for (int i = 0; i < spawnTargetCount; i++)
                    {
                        var mon = waveData.GetSummonMonster();

                        var spawnedActor = _monsterSpawnController.SpawnMonster(mon);
                        if (spawnedActor is not null)
                            waveData.SpawnedCount++;
                        else
                        {
                            DebugUtils.Log("몬스터 최대 수량 제한 등으로 소환 일시 중단");
                            break;
                        }
                    }

                    waveData.RefreshSpawnTimer();
                }

                await UniTask.Yield(PlayerLoopTiming.Update, spawnCts.Token);
            }
        }
        
        
        private void StopSpawning()
        {
            if (spawnCts != null)
            {
                spawnCts.Cancel();
                spawnCts.Dispose();
                spawnCts = null;
            }
        }
        
        private void OnDestroy()
        {
            StopSpawning();
        }
        
        private void SendWaveStartEvent(int currentWave, int interval)
        {
            _nextWaveTime = Time.time + interval;
            GameEvents.SendEvent(new WaveStartEvent(currentWave, _nextWaveTime));
        }
    }
}