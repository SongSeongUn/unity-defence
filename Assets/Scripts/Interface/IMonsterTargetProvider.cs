using UnityEngine;
using System.Collections.Generic;

namespace Interface
{
    public interface IMonsterTargetProvider
    {
        MonsterActor FindNearest(float yPos);
        List<MonsterActor> FindNearestChain(Vector2 pos, int count, float maxDistance = 3);
    }
}