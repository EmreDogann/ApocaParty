using System;
using Audio;
using UnityEngine;

namespace GuestRequests.Jobs
{
    public class FixBunting : Job
    {
        public float Duration = 1.0f;
        public SpriteRenderer BuntingSprite;
        public Sprite FixedSprite;
        [SerializeField] private Transform _playbackPosition;
        public AudioSO FixAudio;

        public static event Action BuntingFixed;

        public override void Exit()
        {
            if (_playbackPosition)
            {
                FixAudio.Play(_playbackPosition.position);
            }
            else
            {
                FixAudio.Play2D();
            }

            BuntingFixed?.Invoke();
            BuntingSprite.sprite = FixedSprite;
        }

        public override float GetProgressPercentage()
        {
            return Mathf.Clamp01(_currentTime / Duration);
        }

        public override float GetTotalDuration()
        {
            return Duration;
        }
    }
}