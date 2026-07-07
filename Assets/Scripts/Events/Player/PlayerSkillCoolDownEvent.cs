namespace Events.Player
{
    public class PlayerSkillCoolDownEvent: Events.IGameEvent
    {
        public int WeaponNo;
        public float CoolDownTime;
        public float Duration;
        
        public PlayerSkillCoolDownEvent(int weaponNo, float coolDownTime, float duration = 0f)
        {
            WeaponNo = weaponNo;
            CoolDownTime = coolDownTime;
            Duration = duration;
        }
    }
}
