namespace Events.Player
{
    public class PlayerDamagedEvent : IGameEvent
    {
        public BaseActor Actor { get; }
        
        public PlayerDamagedEvent(BaseActor actor)
        {
            this.Actor = actor;
        }
    }
}