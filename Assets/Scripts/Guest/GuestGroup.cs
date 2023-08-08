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
        private Action _currentCallback;
        private bool _sitDownOnArrive;

        public event Action OnGuestsSitDown;

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

            _sitDownOnArrive = true;
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
                _currentCallback?.Invoke();
                _currentCallback = null;

                if (!_sitDownOnArrive)
                {
                    return;
                }

                if (arrivalConversation != null)
                {
                    DialogueManager.Instance.OpenDialogue(arrivalConversation.messages, () => SitDown(true));
                }
                else
                {
                    SitDown(true);
                }

                _arrivalTriggered = false;
            }
        }

        public void SetSitDownOnArrive(bool shouldSitDown)
        {
            _sitDownOnArrive = shouldSitDown;
        }

        public void Arrive(List<Transform> arrivalSpots, Action callback = null)
        {
            _currentCallback = callback;

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

        public void SitDown(bool aiActiveOnSitdown)
        {
            var tableSeats = DiningTableSupplier.GetAvailableSeats(_guests.Count);

            if (tableSeats.Count >= _guests.Count)
            {
                for (int i = 0; i < _guests.Count; i++)
                {
                    _guests[i].AssignTableSeat(tableSeats[i], true);
                }
            }

            StartCoroutine(WaitForGuestSitDown(aiActiveOnSitdown));
        }

        public void Tutorial_ForceSitDown()
        {
            for (int i = 0; i < _guests.Count; i++)
            {
                if (!_guests[i].IsSittingAtSeat())
                {
                    _guests[i].WarpToSeat();
                }
            }
        }

        private IEnumerator WaitForGuestSitDown(bool activateOnSitdown)
        {
            while (true)
            {
                int guestsSittingDown = 0;
                foreach (GuestAI guest in _guests)
                {
                    if (HasReachedDestination(guest))
                    {
                        guestsSittingDown++;
                    }
                }

                if (guestsSittingDown == _guests.Count)
                {
                    break;
                }

                yield return null;
            }

            if (activateOnSitdown)
            {
                foreach (GuestAI guest in _guests)
                {
                    guest.ActivateAI();
                }
            }

            OnGuestsSitDown?.Invoke();
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