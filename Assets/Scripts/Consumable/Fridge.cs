using System.Collections.Generic;
using GuestRequests.Requests;
using JetBrains.Annotations;
using UnityEngine;

namespace Consumable
{
    public class Fridge : MonoBehaviour
    {
        private class FoodRequestData
        {
            public bool IsAvailable;
            public FoodRequest FoodRequest;
        }

        [SerializeField] private Transform foodRequestsHolder;

        private readonly List<FoodRequestData> _foodRequests = new List<FoodRequestData>();

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
            foreach (FoodRequestData data in _foodRequests)
            {
                data.FoodRequest.OnConsumed += OnFoodConsumed;
            }
        }

        private void OnDisable()
        {
            foreach (FoodRequestData data in _foodRequests)
            {
                data.FoodRequest.OnConsumed -= OnFoodConsumed;
            }
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
            foreach (FoodRequestData data in _foodRequests)
            {
                if (data.IsAvailable && data.FoodRequest.TryStartRequest())
                {
                    data.IsAvailable = false;
                    return data.FoodRequest;
                }
            }

            return null;
        }
    }
}