using System.Collections.Generic;
using Dialogue;
using DiningTable;
using UnityEngine;

namespace Guest
{
    public enum GroupType
    {
        Famine,
        War,
        Pestilence,
        Death
    }

    public class GuestGroup : MonoBehaviour
    {
        [SerializeField] private List<GuestAI> _guests;
        [SerializeField] private ConversationSO arrivalConversation;
        [field: SerializeReference] public GroupType GroupType { get; private set; }

        private bool _arrivalTriggered;

        private void Reset()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform child = transform.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    break;
                }

                GuestAI guest = child.GetComponent<GuestAI>();
                if (guest != null)
                {
                    _guests.Add(guest);
                }
            }
        }

        private void Update()
        {
            if (_arrivalTriggered)
            {
                int guestsArrived = 0;
                foreach (GuestAI guest in _guests)
                {
                    if (guest.navMeshAgent.pathPending)
                    {
                        continue;
                    }

                    if (!(guest.navMeshAgent.remainingDistance <= guest.navMeshAgent.stoppingDistance))
                    {
                        continue;
                    }

                    if (!guest.navMeshAgent.hasPath || guest.navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        guestsArrived++;
                    }
                }

                if (guestsArrived == _guests.Count)
                {
                    DialogueManager.Instance.OpenDialogue(arrivalConversation.messages, SitDown);
                    _arrivalTriggered = false;
                }
            }
        }

        public void Arrive(List<Transform> arrivalSpots)
        {
            int index = 0;
            foreach (GuestAI guestAI in _guests)
            {
                guestAI.transform.parent.gameObject.SetActive(true);
                guestAI.transform.gameObject.SetActive(true);
                guestAI.enabled = true;

                guestAI.SetDestination(arrivalSpots[index].position);
                index++;
            }

            _arrivalTriggered = true;
        }

        public void SitDown()
        {
            var tableSeats = DiningTableSupplier.GetAvailableSeats(_guests.Count);

            if (tableSeats.Count >= _guests.Count)
            {
                for (int i = 0; i < _guests.Count; i++)
                {
                    _guests[i].AssignTableSeat(tableSeats[i]);
                    _guests[i].ActivateAI();
                }
            }
        }
    }
}