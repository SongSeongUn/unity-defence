using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class WallRow : ITableRow
    {
        public int No;
        public int Level;
        public long HP;
        public string Wall_Icon;
        public string Wall_Image;
        public int Key => No;
    }
}
