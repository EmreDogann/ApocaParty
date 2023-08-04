using UnityEngine;

public interface IWaiterTarget
{
    public bool HasUnknownRequest();
    public bool HasConsumable();
    public void WaiterInteracted(IWaiter waiter);
    public void WaiterCancelled();
    public Transform GetDestinationTransform();
    public bool IsAssignedWaiter();
    public void GiveWaiterID(int waiterID);
    public int GetWaiterID();
}