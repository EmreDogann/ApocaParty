using Consumable;

namespace Player
{
    public interface IWaiter
    {
        public IConsumable GetFood();
        public int GetWaiterID();
    }
}