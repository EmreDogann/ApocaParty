using TransformProvider;
using UnityEngine;

namespace Electricity
{
    [RequireComponent(typeof(TransformPair))]
    public class Stove : MonoBehaviour, IElectricalAppliance
    {
        private TransformPair _transformPair;
        private bool _isUsing;

        private void Awake()
        {
            _transformPair = GetComponent<TransformPair>();
        }

        public TransformPair GetTransformPair()
        {
            return _transformPair;
        }

        public bool IsInUse()
        {
            return IsAssigned();
        }

        public bool IsAssigned()
        {
            return _isUsing;
        }

        public void Assign()
        {
            _isUsing = true;
        }

        public void Release()
        {
            _isUsing = false;
        }
    }
}