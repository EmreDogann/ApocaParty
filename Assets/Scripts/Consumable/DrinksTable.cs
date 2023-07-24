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
            public bool IsOnTable;
            public Drink Drink;
        }

        [SerializeField] private RequestInteractable drinksTableInteractable;

        [SerializeField] private GameObject drinkPrefab;
        [SerializeField] private Transform drinksTableCover;

        [SerializeField] private int drinkPoolSize = 12;
        [SerializeField] private int tableCapacity = 6;
        [Min(0.0f)] [SerializeField] private float hSpacing = 0.5f;
        [Min(0.0f)] [SerializeField] private float vSpacing = 0.5f;

        private readonly List<Drink> _allDrinks = new List<Drink>();
        private readonly List<Drink> _drinksOnTable = new List<Drink>();
        private readonly List<Vector2> _drinkPositions = new List<Vector2>();
        public static DrinksTable Instance { get; private set; }

        private Vector2 drinksTableFullScale;

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

            for (int i = 0; i < drinkPoolSize; i++)
            {
                Drink drink = Instantiate(drinkPrefab, transform).GetComponent<Drink>();
                bool onTable = i < tableCapacity;
                _allDrinks.Add(drink);

                if (onTable)
                {
                    _drinksOnTable.Add(drink);
                }
            }

            drinksTableFullScale = drinksTableCover.localScale;
            drinksTableCover.localScale = new Vector3(drinksTableFullScale.x, 0.0f, 0.0f);

            drinksTableInteractable.SetInteractableActive(false);
        }

        private void OnEnable()
        {
            drinksTableInteractable.GetRequest().OnRequestCompleted += RefillDrinks;
            PartyEvent.OnPartyEvent += OnFamineEvent;

            for (int i = 0; i < drinkPoolSize; i++)
            {
                _allDrinks[i].OnClaim += OnDrinkClaim;
            }
        }

        private void OnDisable()
        {
            drinksTableInteractable.GetRequest().OnRequestCompleted -= RefillDrinks;
            PartyEvent.OnPartyEvent -= OnFamineEvent;

            for (int i = 0; i < drinkPoolSize; i++)
            {
                _allDrinks[i].OnClaim -= OnDrinkClaim;
            }
        }

        private void OnDrinkClaim(Drink drink)
        {
            for (int i = _drinksOnTable.Count - 1; i >= 0; i--)
            {
                if (drink == _drinksOnTable[i])
                {
                    drinksTableInteractable.SetInteractableActive(true);
                    _drinksOnTable.RemoveAt(i);
                    if (_drinksOnTable.Count % 2 == 0)
                    {
                        drinksTableCover.localScale += new Vector3(0.0f, drinksTableFullScale.y * (1 / 3.0f), 0.0f);
                    }
                }
            }
        }

        [CanBeNull]
        public Drink TryGetDrink()
        {
            foreach (Drink drink in _drinksOnTable)
            {
                drink.Consume();
                return drink;
            }

            return null;
        }

        public bool IsDrinksTableFull()
        {
            return _drinksOnTable.Count >= tableCapacity;
        }

        private void OnFamineEvent(PartyEventData eventData)
        {
            if (eventData.eventType == PartyEventType.FamineAtDrinks)
            {
                for (int i = _drinksOnTable.Count - 1; i >= 0; i--)
                {
                    _drinksOnTable[i].Consume();
                }

                drinksTableInteractable.SetInteractableActive(true);
            }
        }

        private void RefillDrinks()
        {
            foreach (Drink drink in _allDrinks)
            {
                if (_drinksOnTable.Contains(drink))
                {
                    continue;
                }

                if (drink.IsConsumed())
                {
                    ((IConsumableInternal)drink).ResetConsumable();
                    _drinksOnTable.Add(drink);
                }
            }

            drinksTableCover.localScale = new Vector3(drinksTableFullScale.x, 0.0f, 0.0f);

            drinksTableInteractable.SetInteractableActive(false);
        }
    }
}