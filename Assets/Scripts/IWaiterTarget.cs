using UnityEngine;

public interface IWaiterTarget
{
    public void WaiterInteracted(IWaiter waiter);
    public void TargetCancelled() {}
    public Transform GetDestinationTransform();
    public bool IsAssignedWaiter();
    public void GiveWaiterID(int waiterID);
    public int GetWaiterID();
}