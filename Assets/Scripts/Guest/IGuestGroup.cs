using System;
using System.Collections.Generic;
using UnityEngine;

namespace Guest
{
    public interface IGuestGroup
    {
        public void Arrive(List<Transform> arrivalSpots, Action callback = null);
        public GroupType GetGroupType();
    }
}