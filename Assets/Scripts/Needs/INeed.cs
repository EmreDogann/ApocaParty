namespace Needs
{
    public enum NeedType
    {
        Food,
        Drink,
        Music,
        Movement
    }
    public interface INeed
    {
        public NeedType GetNeedType();
        public float GetNeedReward();
        public bool IsExpired();
    }
}