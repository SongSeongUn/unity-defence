using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class CrowdControlRow : ITableRow
    {
        public int No;
        public int CC_Type;
        public int CC_Time_Rate;
        public int CC_Chance_Rate;
        public int CC_Debuff_Rate;
        public string CC_Image;
        public int Key => No;
    }
}
