namespace EnhancedShift.Player.Core
{
    public class PlayerState
    {
        public bool Ground { get; set; }
        public bool Run { get; set; }
        public bool Walk { get; set; }
        public bool Sit { get; set; }
        public bool Jump { get; set; }
        public bool Fall { get; set; }
        public bool Atk { get; set; }
        public bool Hurt { get; set; }
        public bool Pick { get; set; }
        public bool Roll { get; set; }
        public bool Dead { get; set; }

        public void Reset()
        {
            Run = false;
            Walk = false;
            Sit = false;
            Jump = false;
            Fall = false;
            Atk = false;
            Hurt = false;
            Pick = false;
            Roll = false;
        }

        public void Stop()
        {
            Reset();
            Dead = true;
        }
    }
}