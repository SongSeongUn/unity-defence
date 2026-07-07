using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class TestRow : ITableRow
    {
        public int No;
        public string Name;
        public string Desc;
        public List<float> Damage;
        public List<float> Delay;
        public int Key => No;
    }
}
