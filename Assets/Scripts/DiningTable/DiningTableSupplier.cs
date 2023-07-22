using System.Collections.Generic;
using UnityEngine;

namespace DiningTable
{
    public class DiningTableSupplier : MonoBehaviour
    {
        private readonly List<Table> _tables = new List<Table>();

        public delegate List<TableSeat> GetTableSeatsDelegate(int guestCount);

        public static GetTableSeatsDelegate GetAvailableSeats;

        private void Awake()
        {
            GetAvailableSeats = TryGetAvailableSeats;

            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform child = transform.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    break;
                }

                Table table = child.GetComponent<Table>();
                if (table != null)
                {
                    _tables.Add(table);
                }
            }
        }

        public List<TableSeat> TryGetAvailableSeats(int guestCount)
        {
            foreach (Table table in _tables)
            {
                var tableSeat = table.TryGetAvailableSeats();
                if (tableSeat.Count == guestCount)
                {
                    return tableSeat;
                }
            }

            return null;
        }
    }
}