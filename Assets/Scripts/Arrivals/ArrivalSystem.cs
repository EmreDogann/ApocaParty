using System.Collections.Generic;
using System.Linq;
using Guest;
using MyBox;
using UnityEngine;

namespace Arrivals
{
    public class ArrivalSystem : MonoBehaviour
    {
        [SerializeField] private List<Transform> arrivalSpots;
        [SerializeField] private List<GroupType> arrivalOrder;

        private List<GuestGroup> _guestGroups = new List<GuestGroup>();
        private int _currentIndex;

        private void Awake()
        {
            _guestGroups = FindObjectsOfType<MonoBehaviour>(true)
                .OfType<GuestGroup>()
                .ToList();
            _currentIndex = 0;
        }

        [ButtonMethod]
        public void GuestsArrive()
        {
            foreach (GuestGroup guestGroup in _guestGroups)
            {
                if (guestGroup.GroupType == arrivalOrder[_currentIndex])
                {
                    guestGroup.Arrive(arrivalSpots);
                    _currentIndex++;
                    break;
                }
            }
        }
    }
}