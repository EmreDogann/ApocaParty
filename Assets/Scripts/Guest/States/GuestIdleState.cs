using UnityEngine;

namespace Guest.States
{
    public class GuestIdleState : GuestState
    {
        private float _currentWanderTime;

        public GuestIdleState(GuestAI guest, GuestStateMachine stateMachine) : base(guest, stateMachine) {}

        public override GuestStateID GetID()
        {
            return GuestStateID.Idle;
        }

        public override void Enter()
        {
            guest.image.sprite = guest.actorData.sprite;
            _currentWanderTime = 0.0f;
        }

        public override void Tick()
        {
            guest.needSystem.Tick();

            if (guest.InteractableState.IsHovering)
            {
                guest.transform.localScale = Vector3.one * 1.2f;
            }
            else
            {
                guest.transform.localScale = Vector3.one;
            }

            if (!guest.needSystem.IsSatisfied())
            {
                return;
            }

            _currentWanderTime += Time.deltaTime;

            if (_currentWanderTime >= guest.wanderCheckFrequency)
            {
                float randomChance = Random.Range(0.0f, 1.0f);
                if (randomChance <= guest.chanceToWander)
                {
                    _stateMachine.ChangeState(GuestStateID.Wander);
                }

                _currentWanderTime = 0.0f;
            }
        }

        public override void Exit() {}
    }
}