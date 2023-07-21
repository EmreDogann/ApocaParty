using TransformProvider;

namespace GuestRequests.Requests
{
    public interface IJobOwner
    {
        public TransformHandle TryGetTransformHandle(ITransformProvider transformProvider);
        public void ReturnTransformHandle(ITransformProvider transformProvider);
        public void RegisterTransformProvider(ITransformProvider transformProvider);
    }
}