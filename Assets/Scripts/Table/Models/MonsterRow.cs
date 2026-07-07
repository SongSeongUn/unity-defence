using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class MonsterRow : ITableRow
    {
        public int No;
        public int Grade;
        public int Attack;
        public int Attack_Speed_Rate;
        public int Move_Speed_Rate;
        public List<int> Buff;
        public long HP;
        public string Monster_Image;
        public int Key => No;
    }
}
