using System.Collections.Generic;

namespace Table.Models
{
    [System.Serializable]
    public class PlayerSkillUpgradeRow : ITableRow
    {
        public int No;
        public int Skill_No;
        public int Skill_Requirement;
        public int Description;
        public int Skill_Chance;
        public string SKILL_UPGRADE_TYPE;
        public string Hit_Type;
        public int Skill_Add_Count;
        public string Effect_Type;
        public int Effect_Add_Count;
        public int Skill_Speed_Add_Rate;
        public int Cooldown_Sub_Rate;
        public int Skill_Damage_Add_Rate;
        public int Effect_Damage_Add_Rate;
        public int Effect_Add_Duration;
        public string CC_Type;
        public int CC_Add_Rate;
        public int Stat_Add_Rate;
        public int Key => No;
    }
}
