using Audio;
using Needs;
using PartyEvents;
using TransformProvider;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class Cook : Job
    {
        [SerializeField] private AudioSO _cookingAudio;
        [SerializeField] private AudioSO _burningAudio;
        [SerializeField] private KitchenTopProvider _stovePositionProvider;
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        [SerializeField] private float _fireChance = 0.05f;
        [SerializeField] private float _fireCheckFrequency = 0.4f;
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        [SerializeField] private PartyEvent foodBurningEvent;
        [SerializeField] private ParticleSystem fireParticleSystem;
        [SerializeField] private SpriteRenderer requestSpriteRenderer;
        [SerializeField] private Sprite cookedFoodIcon;
        public float Duration = 1.0f;

        private float _currentCookTime;
        private bool _isFoodBurning;
        private bool _hasFoodAlreadyBurned;
        private NeedMetrics rewardTarget;
        private TransformPair transformPair;

        public override void Enter(IRequestOwner owner, ref NeedMetrics metrics)
        {
            base.Enter(owner, ref metrics);
            transformPair =
                _stovePositionProvider.GetTransformPair(JobOwner.TryGetTransformHandle(_stovePositionProvider));
            _cookingAudio.Play(transformPair.GetChildTransform().position);

            _currentCookTime = 0.0f;
            _isFoodBurning = false;
            _hasFoodAlreadyBurned = false;

            rewardTarget = metrics;

            _stovePositionProvider.TurnOnAppliance(JobOwner.TryGetTransformHandle(_stovePositionProvider));
        }

        public override void Tick(float deltaTime, IRequestOwner owner, ref NeedMetrics metrics)
        {
            if (!_isFoodBurning)
            {
                base.Tick(deltaTime, owner, ref metrics);
                _currentCookTime += deltaTime;

                if (!_hasFoodAlreadyBurned && _currentCookTime > _fireCheckFrequency)
                {
                    _currentCookTime = 0.0f;
                    if (Random.Range(0.0f, 1.0f) < _fireChance)
                    {
                        _isFoodBurning = true;
                        _hasFoodAlreadyBurned = true;

                        fireParticleSystem.Play();
                        foodBurningEvent?.TriggerEvent();

                        _cookingAudio.Stop();
                        _burningAudio.Play(transformPair.GetChildTransform().position);
                    }
                }
                else
                {
                    metrics = rewardTarget * GetProgressPercentage(owner);
                }
            }
        }

        public override void Exit(IRequestOwner owner, ref NeedMetrics metrics)
        {
            _cookingAudio.Stop();
            requestSpriteRenderer.sprite = cookedFoodIcon;

            _stovePositionProvider.TurnOffAppliance(JobOwner.TryGetTransformHandle(_stovePositionProvider));
            JobOwner.ReturnTransformHandle(_stovePositionProvider);
        }

        public override void FailJob(IRequestOwner owner)
        {
            fireParticleSystem.Stop();
            _burningAudio.Stop();
        }

        public override float GetProgressPercentage(IRequestOwner owner)
        {
            return Mathf.Clamp01(_currentTime / Duration);
        }

        public override float GetTotalDuration(IRequestOwner owner)
        {
            return Duration;
        }

        public override bool IsFailed(IRequestOwner owner)
        {
            return _hasFoodAlreadyBurned;
        }
    }
}