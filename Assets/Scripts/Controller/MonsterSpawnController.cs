using UnityEngine;
using System;
using Battle;

public class MonsterSpawnController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private BoxCollider2D spawnAreaCollider;
    [SerializeField] private MonoBehaviour registryObject;
    [SerializeField] private WallActor _wallActor;

    private Interface.IMonsterRegistry monsterRegistry;
    
    private void Awake()
    {
        // 인스펙터에서 할당 안 했으면 자동으로 찾기
        if (spawnAreaCollider == null)
            spawnAreaCollider = GetComponent<BoxCollider2D>();
        
        monsterRegistry = registryObject as Interface.IMonsterRegistry;
    }

    public MonsterActor SpawnMonster(MonsterConfig config)
    {
        Vector3 spawnPos = GetRandomPosition();

        MonsterActor monsterObj = ObjectPoolManager.Instance.Get<MonsterActor>(config.Tag, spawnPos);

        if (monsterObj == null)
        {
            throw new Exception($"[SpawnController] {config.Tag} 소환 실패! 풀을 확인하세요.");
        }

        monsterObj.Init(config, _wallActor);
        monsterRegistry.Register(monsterObj); // MonsterTracker 등록
        return monsterObj;
    }

    public Exception ReturnMonster(MonsterActor mon)
    {
        monsterRegistry.UnRegister(mon);
        ObjectPoolManager.Instance.Return(mon.MonsterConfig.Tag, mon);
        return null;
    }


    public Vector3 GetRandomPosition()
    {
        Bounds bounds = spawnAreaCollider.bounds;

        float x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        float y = UnityEngine.Random.Range(bounds.min.y, bounds.max.y);

        // Z값은 0으로 고정 (2D 게임)
        return new Vector3(x, y, 0f);
    }

    private void OnDrawGizmos()
    {
        if (spawnAreaCollider != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawCube(spawnAreaCollider.bounds.center, spawnAreaCollider.bounds.size);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(spawnAreaCollider.bounds.center, spawnAreaCollider.bounds.size);
        }
    }
}