namespace Electricity
{
    public interface IElectricalAppliance
    {
        public bool IsInUse();
        public void TurnOn();
        public void TurnOff();
    }
}