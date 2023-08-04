using System;
using Audio;
using Electricity;
using PartyEvents;
using TransformProvider;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GuestRequests.Jobs
{
    public class Cook : Job
    {
        [SerializeField] private AudioSO _cookingAudio;
        [SerializeField] private KitchenTopProvider _stovePositionProvider;
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        [SerializeField] private float _fireChance = 0.05f;
        [SerializeField] private float _fireCheckFrequency = 0.4f;
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        [SerializeField] private PartyEvent _foodBurningEvent;
        [SerializeField] private SpriteRenderer _requestSpriteRenderer;
        [SerializeField] private Sprite _cookedFoodIcon;
        public float Duration = 1.0f;

        private float _currentCookTime;
        private bool _isFoodBurning;
        private bool _hasFoodAlreadyBurned;
        private TransformPair _transformPair;

        private AudioHandle _cookingAudioReference;

        public event Action OnFoodCooked;

        public override void Enter()
        {
            base.Enter();
            _transformPair =
                _stovePositionProvider.GetTransformPair(JobOwner.TryGetTransformHandle(_stovePositionProvider));

            _currentCookTime = 0.0f;
            _isFoodBurning = false;
            _hasFoodAlreadyBurned = false;

            _stovePositionProvider.TurnOnAppliance(JobOwner.TryGetTransformHandle(_stovePositionProvider));

            ElectricalBox.OnPowerOutage += OnPowerOutage;
            ElectricalBox.OnPowerFixed += OnPowerFixed;

            _cookingAudioReference = AudioHandle.Invalid;
            if (ElectricalBox.IsPowerOn())
            {
                _cookingAudioReference = _cookingAudio.Play(_transformPair.GetChildTransform().position);
            }
        }

        internal override void OnDestroy()
        {
            ElectricalBox.OnPowerOutage -= OnPowerOutage;
            ElectricalBox.OnPowerFixed -= OnPowerFixed;
        }

        public override void Tick(float deltaTime)
        {
            if (!ElectricalBox.IsPowerOn())
            {
                return;
            }

            if (!_isFoodBurning)
            {
                base.Tick(deltaTime);
                _currentCookTime += deltaTime;

                if (JobOwner.GetRequestOwner().GetOwnerType() == CharacterType.Player || _hasFoodAlreadyBurned ||
                    _currentCookTime <= _fireCheckFrequency)
                {
                    return;
                }

                _currentCookTime = 0.0f;
                if (Random.Range(0.0f, 1.0f) < _fireChance)
                {
                    _isFoodBurning = true;
                    _hasFoodAlreadyBurned = true;

                    _foodBurningEvent?.TriggerEvent();
                    _cookingAudio.Stop(_cookingAudioReference, true, 3.0f);
                }
            }
        }

        public override void Exit()
        {
            _cookingAudio.Stop(_cookingAudioReference, true, 3.0f);
            _requestSpriteRenderer.sprite = _cookedFoodIcon;

            _stovePositionProvider.TurnOffAppliance(JobOwner.TryGetTransformHandle(_stovePositionProvider));
            JobOwner.ReturnTransformHandle(_stovePositionProvider);

            OnFoodCooked?.Invoke();

            ElectricalBox.OnPowerOutage -= OnPowerOutage;
            ElectricalBox.OnPowerFixed -= OnPowerFixed;
        }

        public override void FailJob()
        {
            _stovePositionProvider.TurnOffAppliance(JobOwner.TryGetTransformHandle(_stovePositionProvider));
        }

        public override float GetProgressPercentage()
        {
            return Mathf.Clamp01(_currentTime / Duration);
        }

        public override float GetTotalDuration()
        {
            return Duration;
        }

        public override bool IsFailed()
        {
            return _hasFoodAlreadyBurned;
        }

        private void OnPowerOutage()
        {
            if (!_isFoodBurning)
            {
                _cookingAudio.FadeAudio(_cookingAudioReference, 0.0f, 10.0f);
            }
        }

        private void OnPowerFixed()
        {
            if (!_isFoodBurning)
            {
                _cookingAudio.UnFadeAudio(_cookingAudioReference, 10.0f);
            }
        }
    }
}