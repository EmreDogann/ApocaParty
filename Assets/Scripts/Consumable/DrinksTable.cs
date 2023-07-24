using System.Collections.Generic;
using Interactions.Interactables;
using JetBrains.Annotations;
using PartyEvents;
using UnityEngine;

namespace Consumable
{
    public class DrinksTable : MonoBehaviour
    {
        private class DrinkData
        {
            public bool IsAssigned;
            public Drink Drink;
        }

        [SerializeField] private RequestInteractable drinksTableInteractable;

        [SerializeField] private GameObject drinkPrefab;
        [SerializeField] private int drinkPoolSize = 12;
        [SerializeField] private int tableCapacity = 6;
        [Min(0.0f)] [SerializeField] private float hSpacing = 0.5f;
        [Min(0.0f)] [SerializeField] private float vSpacing = 0.5f;

        private readonly List<DrinkData> _drinks = new List<DrinkData>();
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
                _drinks.Add(new DrinkData
                {
                    IsAssigned = false,
                    Drink = drink
                });

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

            drinksTableInteractable.SetInteractableActive(false);
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
                _drinks[i].Drink.transform.localPosition = position;
            }
        }

        private void OnEnable()
        {
            drinksTableInteractable.GetRequest().OnRequestCompleted += RefillDrinks;
            PartyEvent.OnPartyEvent += OnFamineEvent;

            for (int i = 0; i < drinkPoolSize; i++)
            {
                _drinks[i].Drink.OnClaim += OnDrinkClaim;
            }
        }

        private void OnDisable()
        {
            drinksTableInteractable.GetRequest().OnRequestCompleted -= RefillDrinks;
            PartyEvent.OnPartyEvent -= OnFamineEvent;

            for (int i = 0; i < drinkPoolSize; i++)
            {
                _drinks[i].Drink.OnClaim -= OnDrinkClaim;
            }
        }

        private void OnDrinkClaim()
        {
            drinksTableInteractable.SetInteractableActive(true);
        }

        [CanBeNull]
        public Drink TryGetDrink()
        {
            foreach (DrinkData data in _drinks)
            {
                if (!data.IsAssigned)
                {
                    data.IsAssigned = true;
                    return data.Drink;
                }
            }

            return null;
        }

        public bool IsDrinksTableFull()
        {
            int availableCount = 0;
            foreach (DrinkData data in _drinks)
            {
                if (data.Drink.IsAvailable() && !data.IsAssigned)
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
                foreach (DrinkData data in _drinks)
                {
                    if (data.Drink.IsAvailable())
                    {
                        data.Drink.Consume();
                    }
                }

                drinksTableInteractable.SetInteractableActive(true);
            }
        }

        private void RefillDrinks()
        {
            foreach (Vector2 position in _drinkPositions)
            {
                foreach (DrinkData data in _drinks)
                {
                    if (!data.Drink.IsVisible())
                    {
                        ((IConsumableInternal)data.Drink).ResetConsumable();
                        data.Drink.transform.localPosition = position;
                        break;
                    }
                }
            }

            drinksTableInteractable.SetInteractableActive(false);
        }
    }
}