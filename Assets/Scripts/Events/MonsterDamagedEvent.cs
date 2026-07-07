namespace Events
{
    public class MonsterDamagedEvent : IGameEvent
    {
        public BaseActor Actor { get; }
        
        public MonsterDamagedEvent(BaseActor actor)
        {
            this.Actor = actor;
        }
    }
}