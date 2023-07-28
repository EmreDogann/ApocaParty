using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private Transform door;

        private List<GuestGroup> _guestGroups = new List<GuestGroup>();
        private int _currentIndex;
        [SerializeField] private bool arriveOnStart;

        private void Awake()
        {
            _guestGroups = FindObjectsOfType<MonoBehaviour>(true)
                .OfType<GuestGroup>()
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

            foreach (GuestGroup guestGroup in _guestGroups)
            {
                if (guestGroup.GroupType == arrivalOrder[_currentIndex])
                {
                    guestGroup.Arrive(arrivalSpots, GuestsArrived);
                    _currentIndex++;
                    break;
                }
            }
        }

        private void GuestsArrived()
        {
            door.gameObject.SetActive(true);
        }
    }
}