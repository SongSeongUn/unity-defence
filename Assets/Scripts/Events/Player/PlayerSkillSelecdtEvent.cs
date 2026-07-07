using Table.Models;
namespace Events.Player
{
    public class PlayerSkillSelecdtEvent: IGameEvent
    {
        public PlayerSkillRow SkillRow;
        public PlayerSkillUpgradeRow UpgradeRow;

        public PlayerSkillSelecdtEvent(PlayerSkillRow skillRow, PlayerSkillUpgradeRow upgradeRow)
        {
            SkillRow = skillRow;
            UpgradeRow = upgradeRow;
        }
    }
}