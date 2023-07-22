namespace GuestRequests.Requests
{
    public class PowerRequest : Request
    {
        protected override void Awake()
        {
            base.Awake();
            // OnMusicRequested?.Invoke(_requestedMusic);
        }
    }
}