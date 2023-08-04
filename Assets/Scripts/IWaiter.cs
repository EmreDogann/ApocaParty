using Consumable;

public interface IWaiter
{
    public CharacterType GetWaiterType();
    public IConsumable GetConsumable();
}