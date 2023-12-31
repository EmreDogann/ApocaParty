﻿using System;
using System.Collections;
using System.Collections.Generic;
using DiningTable;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

namespace Guest
{
    public class TutorialGuestGroup : MonoBehaviour, IGuestGroup
    {
        private readonly List<GuestAI> _guests = new List<GuestAI>();
        [field: SerializeReference] public GroupType GroupType { get; private set; }
        [SerializeField] private PlayableDirector director;
        [SerializeField] private bool deactivateOnAwake;

        private Action _currentCallback;
        private List<TableSeat> _availableTableSeats;

        protected void Awake()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                GuestAI guest = transform.GetChild(i).GetComponent<GuestAI>();
                if (guest != null)
                {
                    if (deactivateOnAwake)
                    {
                        guest.transform.gameObject.SetActive(false);
                    }

                    _guests.Add(guest);
                }
            }
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

            _currentCallback?.Invoke();
            _currentCallback = null;

            _availableTableSeats = DiningTableSupplier.GetAvailableSeats(_guests.Count);
            director.Play();
        }

        public void SitDownGuests(bool activateOnSitdown)
        {
            director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            _availableTableSeats ??= DiningTableSupplier.GetAvailableSeats(_guests.Count);

            if (_availableTableSeats.Count <= 0)
            {
                return;
            }

            foreach (GuestAI guest in _guests)
            {
                guest.AssignTableSeat(_availableTableSeats[^1], true);
                _availableTableSeats.RemoveAt(_availableTableSeats.Count - 1);
            }

            StartCoroutine(WaitForGuestSitDown(activateOnSitdown));
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

            director.playableGraph.GetRootPlayable(0).SetSpeed(1);
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