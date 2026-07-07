using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class BuffRow : ITableRow
    {
        public int No;
        public string Buff_Type;
        public int Buff_Time;
        public int Buff_Effect_Rate;
        public string Buff_Icon;
        public string Buff_Image;
        public int Key => No;
    }
}
