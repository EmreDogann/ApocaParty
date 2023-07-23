using Consumable;

namespace Player
{
    public interface IWaiter
    {
        public IConsumable GetFood();
        public void FinishInteraction();
        public int GetWaiterID();
    }
}