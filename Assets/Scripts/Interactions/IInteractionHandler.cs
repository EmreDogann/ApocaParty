namespace Interactions
{
    public interface IInteractionHandler
    {
        public InteractableBase CheckForInteraction();

        public bool WasInteractedThisFrame();
    }
}