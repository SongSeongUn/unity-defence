using System;
using System.Collections.Generic;

using Interface;
using UnityEngine;


namespace System
{
    public class MonsterTracker : MonoBehaviour, IMonsterRegistry, IMonsterTargetProvider
    {
        private readonly List<MonsterActor> _monsters = new();

        public void Register(MonsterActor mon)
        {
            if (!_monsters.Contains(mon))
                _monsters.Add(mon);
        }

        public void UnRegister(MonsterActor mon)
        {
            if (!_monsters.Contains(mon))
                throw new Exception($"{mon.name} 몬스터 회수 실패");
            
            _monsters.Remove(mon);
        }
        
        public MonsterActor FindNearest(float yPos)
        {
            MonsterActor nearest = null;
            float minDistance = Mathf.Infinity;
            
            foreach (var monster in _monsters)
            {
                if (!monster.IsInMap) continue;
                
                float dist = (monster.transform.position.y - yPos);
            
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = monster;
                }
            }

            return nearest;
        }

        public List<MonsterActor> FindNearestChain(Vector2 pos, int count, float maxDistance = 3f)
        {
            List<MonsterActor> chainTargets = new();
            HashSet<MonsterActor> visited = new();
            
            for (int i = 0; i < count; i++)
            {
                MonsterActor nearest = null;
                float minDistance = maxDistance;
                foreach (var monster in _monsters)
                {
                    if (visited.Contains(monster)) continue;
                
                    if (!monster.IsInMap) continue;
                    
                    if (monster.transform.position.Equals(pos)) continue;
                
                    float dist = Vector2.Distance(monster.transform.position, pos);
            
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        nearest = monster;
                    }
                }
                
                
                if (nearest == null)
                    break;
                
                chainTargets.Add(nearest);
                visited.Add(nearest);
                
                pos = nearest.transform.position;
            }
           
            return chainTargets;
        }
    }
}