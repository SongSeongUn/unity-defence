using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class BattleMapRow : ITableRow
    {
        public int No;
        public int Map_Wave;
        public int Wave_Time;
        public string Map_Image;
        public int Key => No;
    }
}
