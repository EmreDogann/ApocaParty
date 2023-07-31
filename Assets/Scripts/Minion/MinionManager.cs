using System.Collections.Generic;
using System.Linq;
using Audio;
using Interactions;
using UnityEngine;

namespace Minion
{
    public class MinionManager : MonoBehaviour
    {
        [SerializeField] private AudioSO errorSound;

        private List<MinionAI> _minions;

        private void Awake()
        {
            _minions = GetComponentsInChildren<MinionAI>().ToList();
        }

        private void OnEnable()
        {
            MouseInteraction.OnAltInteract += OnAltInteract;
        }

        private void OnDisable()
        {
            MouseInteraction.OnAltInteract -= OnAltInteract;
        }

        public void SetActiveAll(bool isActive)
        {
            foreach (MinionAI minion in _minions)
            {
                minion.SetActiveMinionAI(isActive);
            }
        }

        public void SetActiveWanderingAll(bool isActive)
        {
            foreach (MinionAI minion in _minions)
            {
                minion.SetActiveWandering(isActive);
            }
        }

        private void OnAltInteract(InteractableBase interactable)
        {
            float minDistance = Mathf.Infinity;
            MinionAI closestMinion = null;
            foreach (MinionAI minion in _minions)
            {
                if (minion.StateMachine.GetCurrentState().GetID() != MinionStateID.Idle)
                {
                    continue;
                }

                float currentDistance =
                    Vector3.SqrMagnitude(minion.transform.position - interactable.transform.position);
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    closestMinion = minion;
                }
            }

            if (closestMinion != null)
            {
                closestMinion.OnInteract(interactable);
            }
            else
            {
                errorSound.Play2D();
            }
        }
    }
}