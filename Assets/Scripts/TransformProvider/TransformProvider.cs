using System;
using UnityEngine;

namespace TransformProvider
{
    public class TransformProvider : MonoBehaviour, ITransformProvider
    {
        private TransformPair _transformPair;
        private TransformHandle _transformPairHandle;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform child = transform.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    break;
                }

                TransformPair transformPair = child.GetComponent<TransformPair>();

                if (transformPair)
                {
                    _transformPair = transformPair;
                    _transformPairHandle = new TransformHandle(Guid.NewGuid().GetHashCode());
                }
            }
        }

        public TransformHandle TryAcquireTransform()
        {
            if (!_transformPair.IsAssigned())
            {
                _transformPair.Assign();
                return _transformPairHandle;
            }

            return null;
        }

        public TransformPair GetTransformPair(TransformHandle transformHandle)
        {
            return _transformPairHandle == transformHandle ? _transformPair : null;
        }

        public void ReturnTransform(TransformHandle transformHandle)
        {
            if (_transformPairHandle == transformHandle)
            {
                _transformPair.Release();
            }
        }
    }
}