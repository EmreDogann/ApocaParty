﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Guest;
using MyBox;
using UnityEngine;

namespace Arrivals
{
    public class ArrivalSystem : MonoBehaviour
    {
        [SerializeField] private List<Transform> arrivalSpots;
        [SerializeField] private List<GroupType> arrivalOrder;
        [SerializeField] private AudioSO guestArrivalAudio;
        [SerializeField] private Transform door;
        [SerializeField] private Transform doorArchway;

        private List<IGuestGroup> _guestGroups = new List<IGuestGroup>();
        private int _currentIndex;
        [SerializeField] private bool arriveOnStart;

        public static event Action OnGuestsArrived;

        private void Awake()
        {
            _guestGroups = FindObjectsOfType<MonoBehaviour>(true)
                .OfType<IGuestGroup>()
                .ToList();
            _currentIndex = 0;
        }

        private void OnEnable()
        {
            DoomsdayTimer.DoomsdayReminder += DoomsdayReminder;
        }

        private void OnDisable()
        {
            DoomsdayTimer.DoomsdayReminder -= DoomsdayReminder;
        }

        private void DoomsdayReminder()
        {
            StartCoroutine(WaitOneFrameForGuestsArrival());
        }

        private IEnumerator WaitOneFrameForGuestsArrival()
        {
            yield return null;

            if (_currentIndex >= _guestGroups.Count)
            {
                yield break;
            }

            RingDoorbell();
            GuestsArrive();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSecondsRealtime(0.25f);
            if (arriveOnStart)
            {
                GuestsArrive();
            }
        }

        [ButtonMethod]
        public void GuestsArrive()
        {
            if (_currentIndex >= _guestGroups.Count)
            {
                return;
            }

            doorArchway.gameObject.SetActive(true);
            door.gameObject.SetActive(false);

            foreach (IGuestGroup guestGroup in _guestGroups)
            {
                if (guestGroup.GetGroupType() == arrivalOrder[_currentIndex])
                {
                    guestGroup.Arrive(arrivalSpots, AfterGuestsArrived);
                    _currentIndex++;
                    break;
                }
            }
        }

        public void RingDoorbell()
        {
            guestArrivalAudio.Play(door.position);
        }

        private void AfterGuestsArrived()
        {
            doorArchway.gameObject.SetActive(false);
            door.gameObject.SetActive(true);
            OnGuestsArrived?.Invoke();
        }
    }
}