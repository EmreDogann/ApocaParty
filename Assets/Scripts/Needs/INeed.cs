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
        public NeedMetrics GetReward();
        public bool IsExpired();
    }
}