using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class BattleExpRow : ITableRow
    {
        public int Level;
        public int Need_Exp;
        public int Nomal_Exp;
        public int Mid_Boss_Exp;
        public int Boss_Exp;
        public int Key => Level;
    }
}
