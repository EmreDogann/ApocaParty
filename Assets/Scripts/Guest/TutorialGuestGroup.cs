using System;
using System.Collections;
using System.Collections.Generic;
using DiningTable;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

namespace Guest
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TutorialGuestGroup : MonoBehaviour, IGuestGroup
    {
        private readonly List<GuestAI> _guests = new List<GuestAI>();
        [field: SerializeReference] public GroupType GroupType { get; private set; }

        private Action currentCallback;
        private PlayableDirector _director;
        private List<TableSeat> _availableTableSeats;

        protected void Awake()
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

            _director = GetComponent<PlayableDirector>();
        }

        private void Update() {}

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

            StartCoroutine(WaitForAllGuestArrival());
        }

        private IEnumerator WaitForAllGuestArrival()
        {
            while (true)
            {
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
                    break;
                }

                yield return null;
            }

            currentCallback?.Invoke();
            currentCallback = null;

            _availableTableSeats = DiningTableSupplier.GetAvailableSeats(_guests.Count);
            _director.Play();
        }

        public void SitDownHenchmen(bool activateOnSitdown)
        {
            _director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            if (_availableTableSeats.Count > 0)
            {
                var henchmen = new List<GuestAI>();
                foreach (GuestAI guest in _guests)
                {
                    if (guest.GuestType == GuestType.Henchmen)
                    {
                        guest.AssignTableSeat(_availableTableSeats[^1], true);
                        henchmen.Add(guest);
                        _availableTableSeats.RemoveAt(_availableTableSeats.Count - 1);
                    }
                }

                StartCoroutine(WaitForGuestSitDown(henchmen, activateOnSitdown));
            }
        }

        private IEnumerator WaitForGuestSitDown(List<GuestAI> guests, bool activateOnSitdown)
        {
            while (true)
            {
                int guestsSittingDown = 0;
                foreach (GuestAI guest in guests)
                {
                    if (HasReachedDestination(guest))
                    {
                        guestsSittingDown++;
                    }
                }

                if (guestsSittingDown == guests.Count)
                {
                    break;
                }

                yield return null;
            }

            yield return new WaitForSeconds(2.0f);

            if (activateOnSitdown)
            {
                foreach (GuestAI guest in guests)
                {
                    guest.ActivateAI();
                }
            }

            _director.playableGraph.GetRootPlayable(0).SetSpeed(1);
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

        public GroupType GetGroupType()
        {
            return GroupType;
        }
    }
}