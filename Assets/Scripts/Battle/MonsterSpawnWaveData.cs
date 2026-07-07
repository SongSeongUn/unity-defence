using System;
using System.Collections.Generic;
using UnityEngine;
using Table.TBL;
using Table.Models;
using static Managers.DataTableManager;

namespace Battle
{
    [System.Serializable]
    public class MonsterConfig
    {
        [Header("▼ 몬스터 정보")]
        [Tooltip("오브젝트 풀 검색용 태그 (예: BasicMonster)")]
        public string Tag; 

        [Header("▼ Monster Stats Override")]
        public int Atk;
        public long MaxHp;
        public long Hp;
    
        [Tooltip("이동 속도 비율")] public int MoveSpeed;
        [Tooltip("공격 속도 비율")] public int AtkSpeed;
        [Tooltip("몬스터 등급")] public int Grade;

        public MonsterConfig(MonsterRow row)
        {
            Atk = row.Attack;
            MaxHp = row.HP;
            Hp = row.HP;
            MoveSpeed = row.Move_Speed_Rate;
            AtkSpeed = row.Attack_Speed_Rate;
            Tag = row.Monster_Image;
            Grade = row.Grade;
        }
    }
    
    public class MonsterSpawnWaveData
    {
        public enum MonsterSpawnType
        {
            Random = 1,
            Sequential = 2,
        }

        private readonly MonsterSpawnRow _spawnRow;
        private readonly List<MonsterConfig> _configs = new();
        private int _currentSpawnMonsterIndex = 0;

        public int SpawnedCount = 0;
        public float NextSpawnTime { get; private set; }
        public int WaveStartInterval { get; private set; }

        // Property
        public MonsterSpawnType SpawnType => (MonsterSpawnType)_spawnRow.Monster_Spawn_Type;
        public int MaxCount => _spawnRow.Monster_Count;
        public int MaxSpawnCount => _spawnRow.Monster_Spawn_Count;
        public int SpawnInterval => _spawnRow.Monster_Count_Time;
        


        public MonsterSpawnWaveData(MonsterSpawnRow spawnRow, int waveStartInterval)
        {
            _spawnRow = spawnRow;
            WaveStartInterval = waveStartInterval;
            foreach (var row in spawnRow.Spawn_Monster)
            {
                var monRow = GetTable<MonsterTable>()?.GetData(row);
                if (monRow == null)
                    throw new Exception($"{row} Monster Not Found");

                _configs.Add(new MonsterConfig(monRow));
            }
        }

        public MonsterConfig GetSummonMonster()
        {
            if (_configs.Count < _spawnRow.Spawn_Monster.Count)
            {
                DebugUtils.LogError("ConfigMonster, it does not match");
                return null;
            }

            if (_spawnRow.Monster_Spawn_Type == (int)MonsterSpawnType.Random)
            {
                return _configs[UnityEngine.Random.Range(0, _spawnRow.Spawn_Monster.Count)];
            }
            else
            {
                if (_currentSpawnMonsterIndex == _spawnRow.Spawn_Monster.Count - 1)
                    _currentSpawnMonsterIndex = 0;
                else
                    _currentSpawnMonsterIndex++;

                return _configs[_currentSpawnMonsterIndex];
            }
        }

        public void RefreshSpawnTimer()
        {
            NextSpawnTime = Time.time + SpawnInterval;
        }
    }
}