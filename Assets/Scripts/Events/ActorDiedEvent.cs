using Common;


namespace Events
{
    public class ActorDiedEvent : IGameEvent
    {
        public BaseActor Actor { get; }
        
        public ActorDiedEvent(BaseActor actor)
        {
            Actor = actor;
        }
    }
}