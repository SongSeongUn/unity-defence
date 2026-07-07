using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class MonsterSpawnRow : ITableRow
    {
        public int No;
        public int Wave;
        public int Map_No;
        public List<int> Spawn_Monster;
        public int Monster_Count;
        public int Monster_Spawn_Count;
        public int Monster_Spawn_Type;
        public int Monster_Count_Time;
        public int Key => No;
    }
}
