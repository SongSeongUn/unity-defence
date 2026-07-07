namespace Events
{
    public class WaveStartEvent : IGameEvent
    {
        public int Wave;
        public float Timer;
        
        public WaveStartEvent(int wave, float time)
        {
            Wave = wave;
            Timer = time;
        }
    }
}