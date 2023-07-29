using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Guest;
using MyBox;
using UI.Components;
using UnityEngine;

namespace Arrivals
{
    public class ArrivalSystem : MonoBehaviour
    {
        [SerializeField] private List<Transform> arrivalSpots;
        [SerializeField] private List<GroupType> arrivalOrder;
        [SerializeField] private AudioSO guestArrivalAudio;
        [SerializeField] private Transform door;

        private List<IGuestGroup> _guestGroups = new List<IGuestGroup>();
        private int _currentIndex;
        [SerializeField] private bool arriveOnStart;

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
            RingDoorbell();
            GuestsArrive();
        }

        private void Start()
        {
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
            door.gameObject.SetActive(true);
        }
    }
}