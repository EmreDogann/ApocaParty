﻿using MyBox;
using UnityEngine;

namespace Audio
{
    public class AudioAutoPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSO audioToPlay;
        [SerializeField] private GameObject attachToGameObject;
        [SerializeField] private bool stopExistingAudio;
        [SerializeField] private bool fadeIn;
        [ConditionalField(nameof(fadeIn))] [SerializeField] private float fadeInDuration = 1.0f;

        private void Start()
        {
            if (stopExistingAudio)
            {
                audioToPlay.StopAll();
            }

            if (attachToGameObject)
            {
                audioToPlay.PlayAttached(attachToGameObject);
            }
            else
            {
                audioToPlay.Play(default, fadeIn, fadeInDuration);
            }
        }

        private void OnDestroy()
        {
            audioToPlay.StopAll();
        }
    }
}