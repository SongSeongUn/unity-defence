using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class PlayerCharacterImageRow : ITableRow
    {
        public int No;
        public string Battle_Stand;
        public string Battle_Win;
        public string Battle_Lose;
        public string Illustration;
        public int Key => No;
    }
}
