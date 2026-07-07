namespace Events.Player
{
    public class PlayerLevelUpEvent: IGameEvent
    {
        public int BeforeLevel;
        public int AfterLevel;
        
        public PlayerLevelUpEvent(int beforeLevel, int afterLevel)
        {
            
        }
    }
}