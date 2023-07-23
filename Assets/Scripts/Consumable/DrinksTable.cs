using System.Collections.Generic;
using GuestRequests.Requests;
using JetBrains.Annotations;
using PartyEvents;
using UnityEngine;

namespace Consumable
{
    public class DrinksTable : MonoBehaviour
    {
        [SerializeField] private GameObject drinkPrefab;
        [SerializeField] private int drinkPoolSize = 12;
        [SerializeField] private int tableCapacity = 6;
        [Min(0.0f)] [SerializeField] private float hSpacing = 0.5f;
        [Min(0.0f)] [SerializeField] private float vSpacing = 0.5f;

        private readonly List<Drink> _drinks = new List<Drink>();
        private readonly List<Vector2> _drinkPositions = new List<Vector2>();
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

            int rowIndex = -1;
            for (int i = 0; i < drinkPoolSize; i++)
            {
                Drink drink = Instantiate(drinkPrefab, transform).GetComponent<Drink>();
                _drinks.Add(drink);

                if (i >= tableCapacity)
                {
                    drink.Hide();
                    continue;
                }

                drink.Show();

                if (i % (tableCapacity / 2) == 0)
                {
                    rowIndex++;
                    rowIndex %= 2;
                }


                Vector2 position = new Vector2(rowIndex * (hSpacing / 2.0f) + i % (tableCapacity / 2) * hSpacing,
                    -rowIndex * vSpacing);
                _drinkPositions.Add(position);
                drink.transform.localPosition = position;
            }
        }

        private void OnValidate()
        {
            if (_drinks.Count <= 0)
            {
                return;
            }

            int rowIndex = -1;
            for (int i = 0; i < drinkPoolSize; i++)
            {
                if (i % (tableCapacity / 2) == 0)
                {
                    rowIndex++;
                    rowIndex %= 2;
                }

                Vector2 position = new Vector2(
                    rowIndex * (hSpacing / 2.0f) + i % (tableCapacity / 2) * hSpacing,
                    -rowIndex * vSpacing
                );
                _drinks[i].transform.localPosition = position;
            }
        }

        private void OnEnable()
        {
            DrinkRefillRequest.OnDrinkRefill += RefillDrinks;
            PartyEvent.OnPartyEvent += OnFamineEvent;
        }

        private void OnDisable()
        {
            DrinkRefillRequest.OnDrinkRefill -= RefillDrinks;
            PartyEvent.OnPartyEvent -= OnFamineEvent;
        }

        [CanBeNull]
        public Drink TryGetDrink()
        {
            foreach (Drink drink in _drinks)
            {
                if (drink.IsOnTable())
                {
                    return drink;
                }
            }

            return null;
        }

        public bool IsDrinksTableFull()
        {
            int availableCount = 0;
            foreach (Drink drink in _drinks)
            {
                if (drink.IsOnTable())
                {
                    availableCount++;
                }
            }

            return availableCount >= tableCapacity;
        }

        private void OnFamineEvent(PartyEventData eventData)
        {
            if (eventData.eventType == PartyEventType.FamineAtDrinks)
            {
                foreach (Drink drink in _drinks)
                {
                    if (drink.IsOnTable())
                    {
                        drink.Consume();
                    }
                }
            }
        }

        private void RefillDrinks()
        {
            foreach (Vector2 position in _drinkPositions)
            {
                foreach (Drink drink in _drinks)
                {
                    if (!drink.IsVisible())
                    {
                        ((IConsumableInternal)drink).ResetConsumable();
                        drink.transform.localPosition = position;
                        break;
                    }
                }
            }
        }
    }
}