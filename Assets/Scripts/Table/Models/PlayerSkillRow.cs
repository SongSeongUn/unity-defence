using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class PlayerSkillRow : ITableRow
    {
        public int No;
        public int Name;
        public int Description;
        public string Hit_Type;
        public int Skill_Count;
        public string Effect_Type;
        public int Effect_Count;
        public int Skill_Speed;
        public int Cooldown;
        public int Skill_Damage;
        public int Effect_Damage;
        public int Effect_Duration;
        public int Dot_Damage_Inteval;
        public string CC_Type;
        public int CC_Rate;
        public string Skill_Icon;
        public string Prefab;
        public string Fx;
        public int Key => No;
    }
}
