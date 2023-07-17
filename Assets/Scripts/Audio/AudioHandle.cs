namespace Audio
{
    public struct AudioHandle
    {
        public static AudioHandle Invalid = new AudioHandle(-1, null);

        internal int ID;
        internal AudioSO AudioCue;

        public AudioHandle(int id, AudioSO audioCue)
        {
            ID = id;
            AudioCue = audioCue;
        }

        public override bool Equals(object obj)
        {
            return obj is AudioHandle x && ID == x.ID && AudioCue == x.AudioCue;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode() ^ AudioCue.GetHashCode();
        }

        public static bool operator ==(AudioHandle x, AudioHandle y)
        {
            return x.ID == y.ID && x.AudioCue == y.AudioCue;
        }

        public static bool operator !=(AudioHandle x, AudioHandle y)
        {
            return !(x == y);
        }
    }
}