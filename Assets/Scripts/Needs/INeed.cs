using Dialogue;

namespace Needs
{
    public enum NeedType
    {
        None,
        Food,
        Drink,
        Music,
        Movement
    }
    public interface INeed
    {
        public NeedType GetNeedType();
        public NeedMetrics GetPunishment();
        public bool IsExpired();
        public RandomConversationSO GetRandomConversations();
    }
}