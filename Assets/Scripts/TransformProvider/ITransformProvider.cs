using JetBrains.Annotations;

namespace TransformProvider
{
    public interface ITransformProvider
    {
        public bool IsTransformAvailable();
        [CanBeNull] public TransformHandle TryAcquireTransform();
        public TransformPair GetTransformPair(TransformHandle transformHandle);
        public void ReturnTransform(TransformHandle transformHandle);
    }
}