using UnityEngine;
using UnityEngine.AI;

namespace Guest.States
{
    public class GuestWanderState : GuestState
    {
        private const float DistanceThreshold = 0.1f;
        private const float WanderWaitTime = 3.0f;
        private const float SearchRadius = 3.0f;
        private float _currentWanderTime;

        public GuestWanderState(GuestAI guest, GuestStateMachine stateMachine) : base(guest, stateMachine) {}

        public override GuestStateID GetID()
        {
            return GuestStateID.Wander;
        }

        public override void Enter()
        {
            guest.SetDestination(RandomNavmeshLocation(guest.transform.position, SearchRadius));
            _currentWanderTime = 0.0f;
        }

        public override void Tick()
        {
            if (Vector3.SqrMagnitude(guest.transform.position - guest.navMeshAgent.destination) <
                DistanceThreshold * DistanceThreshold)
            {
                _currentWanderTime += Time.deltaTime;
            }

            if (_currentWanderTime >= WanderWaitTime)
            {
                _currentWanderTime = 0.0f;
                guest.SetDestination(RandomNavmeshLocation(guest.transform.position, SearchRadius));
            }
        }

        public override void Exit() {}

        private Vector3 RandomNavmeshLocation(Vector3 position, float radius)
        {
            Vector3 finalPosition = position;
            for (int i = 0; i < 30; i++)
            {
                Vector3 randomDirection = ClampMagnitude(Random.insideUnitCircle * radius, Mathf.Infinity, 2.0f);
                randomDirection += position;

                if (!NavMesh.Raycast(position, randomDirection, out NavMeshHit raycastHit, guest.navMeshAgent.areaMask))
                {
                    finalPosition = raycastHit.position;
                    break;
                }
            }

            finalPosition.z = 0;
            return finalPosition;
        }

        private Vector3 ClampMagnitude(Vector3 v, float max, float min)
        {
            double sm = v.sqrMagnitude;
            if (sm > max * (double)max)
            {
                return v.normalized * max;
            }

            if (sm < min * (double)min)
            {
                return v.normalized * min;
            }

            return v;
        }
    }
}