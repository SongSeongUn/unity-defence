using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class StringRow : ITableRow
    {
        public int No;
        public string Kr;
        public string En;
        public int Key => No;
    }
}
