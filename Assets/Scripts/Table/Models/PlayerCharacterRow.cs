using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class PlayerCharacterRow : ITableRow
    {
        public int No;
        public int Attack;
        public int Skill_Base_1;
        public int Skill_Base_2;
        public int Character_Image;
        public int Key => No;
    }
}
