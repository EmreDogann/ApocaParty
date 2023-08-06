using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Timeline.Playables
{
    [Serializable]
    public class ExposedReferenceHolder<T> where T : Object
    {
        public ExposedReference<T> exposedReference;
    }
}