using Audio;
using Electricity;
using PartyEvents;
using TransformProvider;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class Cook : Job
    {
        [SerializeField] private AudioSO _cookingAudio;
        [SerializeField] private AudioSO _burningStartAudio;
        [SerializeField] private AudioSO _burningAudio;
        [SerializeField] private KitchenTopProvider _stovePositionProvider;
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        [SerializeField] private float _fireChance = 0.05f;
        [SerializeField] private float _fireCheckFrequency = 0.4f;
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        [SerializeField] private PartyEvent _foodBurningEvent;
        [SerializeField] private ParticleSystem _fireParticleSystem;
        [SerializeField] private ParticleSystem _badHighlightParticleSystem;
        [SerializeField] private SpriteRenderer _requestSpriteRenderer;
        [SerializeField] private Sprite _cookedFoodIcon;
        public float Duration = 1.0f;

        private float _currentCookTime;
        private bool _isFoodBurning;
        private bool _hasFoodAlreadyBurned;
        private TransformPair _transformPair;
        private bool _isPowerOut;

        public override void Enter()
        {
            base.Enter();
            _transformPair =
                _stovePositionProvider.GetTransformPair(JobOwner.TryGetTransformHandle(_stovePositionProvider));
            _cookingAudio.Play(_transformPair.GetChildTransform().position);

            _currentCookTime = 0.0f;
            _isFoodBurning = false;
            _hasFoodAlreadyBurned = false;
            _isPowerOut = false;

            _stovePositionProvider.TurnOnAppliance(JobOwner.TryGetTransformHandle(_stovePositionProvider));
            ElectricalBox.OnPowerOutage += OnPowerOutage;
            ElectricalBox.OnPowerFixed += OnPowerFixed;
        }

        public override void Tick(float deltaTime)
        {
            if (_isPowerOut)
            {
                return;
            }

            if (!_isFoodBurning)
            {
                base.Tick(deltaTime);
                _currentCookTime += deltaTime;

                if (JobOwner.GetRequestOwner().GetOwnerType() == OwnerType.Player || _hasFoodAlreadyBurned ||
                    _currentCookTime <= _fireCheckFrequency)
                {
                    return;
                }

                _currentCookTime = 0.0f;
                if (Random.Range(0.0f, 1.0f) < _fireChance)
                {
                    _isFoodBurning = true;
                    _hasFoodAlreadyBurned = true;

                    _badHighlightParticleSystem.Play();
                    _fireParticleSystem.Play();
                    _foodBurningEvent?.TriggerEvent();

                    _cookingAudio.Stop(true, 3.0f);
                    _burningStartAudio.Play(_transformPair.GetChildTransform().position);
                    _burningAudio.Play(_transformPair.GetChildTransform().position, true, 3.0f);
                }
            }
        }

        public override void Exit()
        {
            _cookingAudio.Stop(true, 3.0f);
            _requestSpriteRenderer.sprite = _cookedFoodIcon;

            _stovePositionProvider.TurnOffAppliance(JobOwner.TryGetTransformHandle(_stovePositionProvider));
            JobOwner.ReturnTransformHandle(_stovePositionProvider);

            ElectricalBox.OnPowerOutage -= OnPowerOutage;
            ElectricalBox.OnPowerFixed -= OnPowerFixed;
        }

        public override void FailJob()
        {
            _stovePositionProvider.TurnOffAppliance(JobOwner.TryGetTransformHandle(_stovePositionProvider));
            _badHighlightParticleSystem.Stop();
            _fireParticleSystem.Stop();
            _burningAudio.Stop(true, 5.0f);
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
                _cookingAudio.FadeAudio(0.0f, 10.0f);
            }

            _isPowerOut = true;
        }

        private void OnPowerFixed()
        {
            if (!_isFoodBurning)
            {
                _cookingAudio.UnFadeAudio(10.0f);
            }

            _isPowerOut = false;
        }
    }
}