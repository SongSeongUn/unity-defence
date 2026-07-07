using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class MonsterImageRow : ITableRow
    {
        public int No;
        public string Monster_Stand;
        public string Monster_Walk;
        public string Monster_Attack;
        public string Monster_Die;
        public int Key => No;
    }
}
