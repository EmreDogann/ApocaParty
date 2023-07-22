using System.Collections.Generic;
using DiningTable;
using UnityEngine;

namespace Guest
{
    public class GuestGroup : MonoBehaviour
    {
        [SerializeField] private List<GuestAI> _guests;

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

        public void Arrive()
        {
            var tableSeats = DiningTableSupplier.GetAvailableSeats(_guests.Count);

            if (tableSeats.Count == _guests.Count)
            {
                for (int i = 0; i < _guests.Count; i++)
                {
                    _guests[i].AssignTableSeat(tableSeats[i]);
                }
            }
        }
    }
}