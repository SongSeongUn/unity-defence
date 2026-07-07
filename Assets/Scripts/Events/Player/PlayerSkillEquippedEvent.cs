using Table.Models;

namespace Events.Player
{
    public class PlayerSkillEquippedEvent : Events.IGameEvent
    {
        public int SkillNo;

        public PlayerSkillEquippedEvent(int skillNo)
        {
            SkillNo = skillNo;
        }
    }
}
