using System.Collections.Generic;
using GuestRequests.Requests;
using JetBrains.Annotations;
using UnityEngine;

namespace Consumable
{
    public class DrinksTable : MonoBehaviour
    {
        private readonly List<Drink> _drinks = new List<Drink>();
        public static DrinksTable Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            _drinks.AddRange(transform.GetComponentsInChildren<Drink>(true));
        }

        private void OnEnable()
        {
            DrinkRefillRequest.OnDrinkRefill += RefillDrinks;
        }

        private void OnDisable()
        {
            DrinkRefillRequest.OnDrinkRefill -= RefillDrinks;
        }

        [CanBeNull]
        public Drink TryGetDrink()
        {
            foreach (Drink drink in _drinks)
            {
                if (!drink.IsConsumed())
                {
                    return drink;
                }
            }

            return null;
        }

        private void RefillDrinks()
        {
            foreach (Drink drink in _drinks)
            {
                ((IConsumableInternal)drink).ResetConsumable();
            }
        }
    }
}