﻿using System.Collections.Generic;
using Audio;
using Interactions.Interactables;
using JetBrains.Annotations;
using MyBox;
using PartyEvents;
using UnityEngine;

namespace Consumable
{
    public class DrinksTable : MonoBehaviour
    {
        [Separator("General")]
        [SerializeField] private ParticleSystem emptyTableParticleSystem;
        [SerializeField] private DrinksTableInteractable drinksTableInteractable;
        [SerializeField] private AudioSO refillSound;

        [Separator("Drinks")]
        [SerializeField] private GameObject drinkPrefab;
        [SerializeField] private Transform drinksTableCover;

        [SerializeField] private int drinkPoolSize = 12;
        [SerializeField] private int tableCapacity = 6;

        private List<Drink> _allDrinks;
        private List<Drink> _availableDrinks;
        public static DrinksTable Instance { get; private set; }

        private Vector2 _drinksTableFullScale;

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

            _allDrinks = new List<Drink>();
            _availableDrinks = new List<Drink>();
            for (int i = 0; i < drinkPoolSize; i++)
            {
                Drink drink = Instantiate(drinkPrefab, transform).GetComponent<Drink>();
                _allDrinks.Add(drink);

                if (i < tableCapacity)
                {
                    _availableDrinks.Add(drink);
                }
            }

            _drinksTableFullScale = drinksTableCover.localScale;
            drinksTableCover.localScale = new Vector3(_drinksTableFullScale.x, 0.0f, 0.0f);

            drinksTableInteractable.SetInteractableActive(true);
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
            _availableDrinks.Remove(drink);

            if (_availableDrinks.Count == 0)
            {
                emptyTableParticleSystem.Play();
            }

            if (_availableDrinks.Count % 2 == 0)
            {
                drinksTableCover.localScale += new Vector3(0.0f, _drinksTableFullScale.y * (1 / 3.0f), 0.0f);
            }
        }

        [CanBeNull]
        public Drink TryGetDrink()
        {
            if (_availableDrinks.Count > 0)
            {
                Drink drink = _availableDrinks[^1];
                return drink;
            }

            return null;
        }

        public void Tutorial_TryGetDrink(int getDrinksCount = 1)
        {
            for (int i = 0; i < getDrinksCount; i++)
            {
                if (_availableDrinks.Count > 0)
                {
                    Drink drink = _availableDrinks[^1];
                    drink.Consume();
                }
            }
        }

        public bool IsDrinkAvailable()
        {
            return _availableDrinks.Count > 0;
        }

        public bool IsDrinksTableFull()
        {
            return _availableDrinks.Count >= tableCapacity;
        }

        private void OnFamineEvent(PartyEventData eventData)
        {
            if (eventData.eventType == PartyEventType.FamineAtDrinks)
            {
                for (int i = _availableDrinks.Count - 1; i >= 0; i--)
                {
                    _availableDrinks[i].Consume();
                }
            }
        }

        [ButtonMethod]
        private void EmptyTable()
        {
            for (int i = _availableDrinks.Count - 1; i >= 0; i--)
            {
                _availableDrinks[i].Consume();
            }
        }

        [ButtonMethod]
        private void RefillDrinks()
        {
            foreach (Drink drink in _allDrinks)
            {
                if (_availableDrinks.Count == tableCapacity)
                {
                    break;
                }

                if (_availableDrinks.Contains(drink))
                {
                    continue;
                }

                if (drink.IsConsumed())
                {
                    ((IConsumableInternal)drink).ResetConsumable();
                    _availableDrinks.Add(drink);
                }
            }

            refillSound.Play(transform.position);

            emptyTableParticleSystem.Stop();
            drinksTableCover.localScale = new Vector3(_drinksTableFullScale.x, 0.0f, 0.0f);
        }
    }
}