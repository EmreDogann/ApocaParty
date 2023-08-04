using System.Collections.Generic;
using Audio;
using GuestRequests.Requests;
using Interactions.Interactables;
using JetBrains.Annotations;
using UnityEngine;

namespace Consumable
{
    public class FoodPile : MonoBehaviour
    {
        private class FoodRequestData
        {
            public bool IsAvailable;
            public FoodRequest FoodRequest;
        }

        [SerializeField] private Transform foodRequestsHolder;
        [SerializeField] private AudioSO foodUnavailableSound;

        private readonly List<FoodRequestData> _foodRequests = new List<FoodRequestData>();

        private bool _isStoveOnFire;

        private void Awake()
        {
            var foodRequests = foodRequestsHolder.GetComponentsInChildren<FoodRequest>();
            foreach (FoodRequest foodRequest in foodRequests)
            {
                _foodRequests.Add(new FoodRequestData
                {
                    IsAvailable = true,
                    FoodRequest = foodRequest
                });
            }
        }

        private void OnEnable()
        {
            FoodRequest.OnFire += OnFire;
            StoveInteractable.OnFireExtinguished += OnFireExtinguished;

            foreach (FoodRequestData data in _foodRequests)
            {
                data.FoodRequest.OnConsumed += OnFoodConsumed;
            }
        }

        private void OnDisable()
        {
            FoodRequest.OnFire -= OnFire;
            StoveInteractable.OnFireExtinguished -= OnFireExtinguished;

            foreach (FoodRequestData data in _foodRequests)
            {
                data.FoodRequest.OnConsumed -= OnFoodConsumed;
            }
        }

        private void OnFire()
        {
            _isStoveOnFire = true;
        }

        private void OnFireExtinguished()
        {
            _isStoveOnFire = false;
        }

        private void OnFoodConsumed(FoodRequest foodRequest)
        {
            FoodRequestData data = _foodRequests.Find(x => x.FoodRequest == foodRequest);
            if (data != null)
            {
                data.IsAvailable = true;
            }
        }

        [CanBeNull]
        public FoodRequest TryGetFood()
        {
            if (_isStoveOnFire)
            {
                return null;
            }

            foreach (FoodRequestData data in _foodRequests)
            {
                if (data.IsAvailable && data.FoodRequest.TryStartRequest())
                {
                    data.IsAvailable = false;
                    return data.FoodRequest;
                }
            }

            foodUnavailableSound.Play2D();
            return null;
        }
    }
}