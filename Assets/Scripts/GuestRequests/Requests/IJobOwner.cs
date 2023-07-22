using TransformProvider;

namespace GuestRequests.Requests
{
    public interface IJobOwner
    {
        public IRequestOwner GetRequestOwner();
        public TransformHandle TryGetTransformHandle(ITransformProvider transformProvider);
        public void ReturnTransformHandle(ITransformProvider transformProvider);
        public void RegisterTransformProvider(ITransformProvider transformProvider);
    }
}