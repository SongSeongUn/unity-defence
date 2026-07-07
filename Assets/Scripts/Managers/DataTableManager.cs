using UnityEngine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Table;
using Table.TBL;


namespace Managers
{
    public static class DataTableManager
    {
        private static Dictionary<Type, TableReader> _tableCache = new();

        public static T GetTable<T>() where T : TableReader, new()
        {
            return GetTableInternal<T>();
        }


        public static async UniTask LoadDataTable(CancellationToken ct = default)
        {
            var tablesToLoad = new List<TableReader>()
            {
                GetTableInternal<StringTable>(),
                GetTableInternal<BattleMapTable>(),
                GetTableInternal<MonsterSpawnTable>(),
                GetTableInternal<MonsterTable>(),
                GetTableInternal<BattleExpTable>(),
                GetTableInternal<PlayerCharacterTable>(),
                GetTableInternal<PlayerSkillTable>(),
                GetTableInternal<PlayerSKillUpgradeTable>(),
            };

            try
            {
                // 모든 테이블을 병렬(Parallel)로 동시에 로드
                await UniTask.WhenAll(tablesToLoad.Select(t => t.InitializeAsync(ct)));
            }
            catch (OperationCanceledException)
            {
                DebugUtils.LogError("테이블 로드가 취소되었습니다.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static T GetTableInternal<T>() where T : TableReader, new()
        {
            if (_tableCache.TryGetValue(typeof(T), out var table)) return table as T;
            T newTbl = new T();
            _tableCache.Add(typeof(T), newTbl);
            return newTbl;
        }

        #if UNITY_EDITOR
            public static T GetEditorTable<T>() where T : TableReader, new()
            {
                CancellationToken ct = default;
                var table = GetTableInternal<T>();
                table.InitializeAsync(ct).Forget();
                return GetTableInternal<T>();
            }
        #endif
    }
}