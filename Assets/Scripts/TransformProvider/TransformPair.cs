using System;
using UnityEngine;

namespace TransformProvider
{
    [Serializable]
    public class TransformPair : MonoBehaviour
    {
        [SerializeField] private Transform transformParent;
        [SerializeField] private Transform transformChild;
        private bool _isAssigned;

        public Transform GetParentTransform()
        {
            return transformParent;
        }

        public Transform GetChildTransform()
        {
            return transformChild;
        }

        public bool IsAssigned()
        {
            return _isAssigned;
        }

        internal void Assign()
        {
            _isAssigned = true;
        }

        internal void Release()
        {
            _isAssigned = false;
        }
    }
}