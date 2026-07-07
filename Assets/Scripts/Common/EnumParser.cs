namespace Common
{
    public enum SkillUpgradeType
    {
        None,
        DamageUpgrade,
        SpawnUpgrade,
        HitCountUpgrade,
        CooldownUpgrade,
        PierceUpgrade,
        RangeUpgrade,
        CCUpgrade,
        DurationUpgrade,
    }

    public enum SkillHitType
    {
        None,
        Single,
        Chain,
    }
    
    public static class EnumParser
    {
        public static SkillUpgradeType ParseUpgradeType(string strType)
        {
            return strType switch
            {
                "DAMAGE_BUFF" => SkillUpgradeType.DamageUpgrade,
                "SPAWN_BUFF" => SkillUpgradeType.SpawnUpgrade,
                "HITCOUNT_BUFF" => SkillUpgradeType.HitCountUpgrade,
                "COOLDOWN_BUFF" => SkillUpgradeType.CooldownUpgrade,
                "PIERCE_BUFF" => SkillUpgradeType.PierceUpgrade,
                "RANGE_BUFF" => SkillUpgradeType.RangeUpgrade,
                "CC_BUFF" => SkillUpgradeType.CCUpgrade,
                "DURATION_BUFF" => SkillUpgradeType.DurationUpgrade,
                _ => SkillUpgradeType.None
            };
        }

        public static SkillHitType ParseHitType(string strType)
        {
            return strType switch
            {
                "SINGLE" => SkillHitType.Single,
                "CHAIN" => SkillHitType.Chain,
                _ => SkillHitType.None
            };
        }
    }
    
}