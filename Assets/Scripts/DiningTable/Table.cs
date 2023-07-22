using System.Collections.Generic;
using UnityEngine;

namespace DiningTable
{
    public class Table : MonoBehaviour
    {
        private readonly List<TableSeat> _tableSeats = new List<TableSeat>();

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                Transform child = transform.GetChild(i);
                if (!child.gameObject.activeSelf)
                {
                    break;
                }

                TableSeat tableSeat = child.GetComponent<TableSeat>();
                if (tableSeat != null)
                {
                    _tableSeats.Add(tableSeat);
                }
            }
        }

        public List<TableSeat> TryGetAvailableSeats()
        {
            var tableSeats = new List<TableSeat>();
            foreach (TableSeat tableSeat in _tableSeats)
            {
                if (tableSeat.IsSeatAvailable())
                {
                    tableSeats.Add(tableSeat);
                }
            }

            return tableSeats;
        }
    }
}