using Dialogue;

namespace Needs
{
    public enum NeedType
    {
        None,
        Food,
        Drink,
        Music
    }
    public interface INeed
    {
        public NeedType GetNeedType();
        public NeedMetrics GetPunishment();
        public bool IsExpired();
        public RandomConversationSO GetRandomConversations();
    }
}