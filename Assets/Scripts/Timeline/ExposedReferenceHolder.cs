using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Timeline
{
    [Serializable]
    public class ExposedReferenceHolder<T> where T : Object
    {
        public ExposedReference<T> exposedReference;
    }
}