using JetBrains.Annotations;

namespace TransformProvider
{
    public interface ITransformProvider
    {
        [CanBeNull] public TransformHandle TryAcquireTransform();
        public TransformPair GetTransformPair(TransformHandle transformHandle);
        public void ReturnTransform(TransformHandle transformHandle);
    }
}