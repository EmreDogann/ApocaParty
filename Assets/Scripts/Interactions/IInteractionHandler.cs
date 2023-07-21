namespace Interactions
{
    public interface IInteractionHandler
    {
        public InteractableBase CheckForInteraction(bool assignmentMode);

        public bool WasInteractedThisFrame();
    }
}