using System;
using System.Collections;
using System.Collections.Generic;
using Dialogue;
using DiningTable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Guest
{
    public enum GroupType
    {
        Famine,
        War,
        Pestilence,
        Death
    }

    public class GuestGroup : MonoBehaviour, IGuestGroup
    {
        private readonly List<GuestAI> _guests = new List<GuestAI>();
        [SerializeField] private ConversationSO arrivalConversation;
        [field: SerializeReference] public GroupType GroupType { get; private set; }

        private bool _arrivalTriggered;
        private Action currentCallback;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                GuestAI guest = transform.GetChild(i).GetComponent<GuestAI>();
                if (guest != null)
                {
                    guest.transform.gameObject.SetActive(false);
                    _guests.Add(guest);
                }
            }
        }

        private void Update()
        {
            if (!_arrivalTriggered)
            {
                return;
            }

            int guestsArrived = 0;
            foreach (GuestAI guest in _guests)
            {
                if (HasReachedDestination(guest))
                {
                    guestsArrived++;
                }
            }

            if (guestsArrived == _guests.Count)
            {
                currentCallback?.Invoke();
                currentCallback = null;

                if (arrivalConversation != null)
                {
                    DialogueManager.Instance.OpenDialogue(arrivalConversation.messages, SitDown);
                }
                else
                {
                    SitDown();
                }

                _arrivalTriggered = false;
            }
        }

        public void Arrive(List<Transform> arrivalSpots, Action callback = null)
        {
            currentCallback = callback;

            int index = 0;
            foreach (GuestAI guestAI in _guests)
            {
                guestAI.transform.parent.gameObject.SetActive(true);
                guestAI.transform.gameObject.SetActive(true);
                guestAI.enabled = true;

                Vector3 randomPosition = new Vector3(Random.Range(0.0f, 0.5f), Random.Range(0.0f, 0.5f), 0.0f);
                guestAI.SetDestination(arrivalSpots[index].position + randomPosition);
                index++;
            }

            _arrivalTriggered = true;
        }

        public GroupType GetGroupType()
        {
            return GroupType;
        }

        public void SitDown()
        {
            var tableSeats = DiningTableSupplier.GetAvailableSeats(_guests.Count);

            if (tableSeats.Count >= _guests.Count)
            {
                for (int i = 0; i < _guests.Count; i++)
                {
                    _guests[i].AssignTableSeat(tableSeats[i], true);
                    _guests[i].ActivateAI();
                }
            }
        }

        private IEnumerator WaitForSitDown(GuestAI guest)
        {
            while (!HasReachedDestination(guest))
            {
                yield return null;
            }

            yield return new WaitForSeconds(2.0f);
            guest.ActivateAI();
        }

        private bool HasReachedDestination(GuestAI guest)
        {
            if (guest.navMeshAgent.pathPending)
            {
                return false;
            }

            if (!(guest.navMeshAgent.remainingDistance <= guest.navMeshAgent.stoppingDistance))
            {
                return false;
            }

            if (!guest.navMeshAgent.hasPath || guest.navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }

            return false;
        }
    }
}