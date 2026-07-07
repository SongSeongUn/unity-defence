using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Table.Models;
using Table.TBL;
using Battle;
using Events;
using static Managers.DataTableManager;

namespace Handler
{
    public class StageWaveHandler : MonoBehaviour
    {
        private BattleMapRow _battleMapRow;
        private List<MonsterSpawnRow> _monsterSpawnRows;

        public void Init(int stageNo, Action<List<MonsterSpawnWaveData>> cb)
        {
            _battleMapRow = GetTable<BattleMapTable>()?.GetData(stageNo);
            if (_battleMapRow == null) throw new Exception("BattleMapData is Null");

            _monsterSpawnRows = GetTable<MonsterSpawnTable>()?.GetStageData(_battleMapRow.No);
            if (_monsterSpawnRows == null || _monsterSpawnRows.Count == 0) throw new Exception("MonsterSpawnDataList is Null Or Empty");

            InitAsync(cb).Forget();
        }

        private async UniTask InitAsync(Action<List<MonsterSpawnWaveData>> cb)
        {
            try
            {
                HashSet<int> loadTask = new HashSet<int>();
                List<UniTask> poolInitTasks = new List<UniTask>(); // 병렬 로드
                
                List<MonsterSpawnWaveData> waveDatas = new();
                for (int i = 0; i < _monsterSpawnRows?.Count; i++)
                {
                    var spawnRow = _monsterSpawnRows[i];
                    waveDatas.Add(new MonsterSpawnWaveData(spawnRow, _battleMapRow.Wave_Time));

                    foreach (var mon in spawnRow.Spawn_Monster)
                    {
                        if (loadTask.Contains(mon))
                            continue;

                        var monRow = GetTable<MonsterTable>()?.GetData(mon);
                        if (monRow == null)
                            throw new Exception($"{mon} Monster Not Found");

                        loadTask.Add(mon);
                        var task = ObjectPoolManager.Instance.InitObjectPool<MonsterActor>(
                            $"Monster/{monRow.Monster_Image}.prefab");
                        poolInitTasks.Add(task);
                    }
                }

                // 모아둔 모든 오브젝트 풀 생성을 병렬(동시)로 처리
                if (poolInitTasks.Count > 0)
                {
                    await UniTask.WhenAll(poolInitTasks);
                }

                cb?.Invoke(waveDatas);
            }
            catch (Exception e)
            {
                DebugUtils.LogError(e.Message);
            }
        }
    }
}