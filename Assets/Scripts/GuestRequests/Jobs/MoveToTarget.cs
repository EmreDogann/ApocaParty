using UnityEngine;

namespace GuestRequests.Jobs
{
    public class MoveToTarget : Job
    {
        public Transform target;
        public MoveToTarget(string name) : base(name) {}
    }
}