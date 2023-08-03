using UnityEngine;

public interface IWaiterTarget
{
    public bool HasRequest();
    public bool HasConsumable();
    public void WaiterInteracted(IWaiter waiter);
    public void TargetCancelled() {}
    public Transform GetDestinationTransform();
    public bool IsAssignedWaiter();
    public void GiveWaiterID(int waiterID);
    public int GetWaiterID();
}