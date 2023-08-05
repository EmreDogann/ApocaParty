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
        public void UpdateTimer(float deltaTime);
        public void ResetNeed();
        public NeedType GetNeedType();
        public NeedMetrics GetPunishment();
        public float GetTimerProgress();
        public bool IsExpired();
        public RandomConversationSO GetRandomConversations();
    }
}