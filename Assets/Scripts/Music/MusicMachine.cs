using Audio;
using DG.Tweening;
using Electricity;
using Events;
using GuestRequests.Requests;
using PartyEvents;
using UI.Components;
using UnityEngine;

namespace Music
{
    [RequireComponent(typeof(MusicMachineBreaksEvent), typeof(MusicRequest))]
    public class MusicMachine : MonoBehaviour
    {
        [Range(0.0f, 1.0f)] [SerializeField] private float machineBreakChance;
        [SerializeField] private float machineBreakCheckFrequency;
        [SerializeField] private float machineBreakCooldown;
        [SerializeField] private AudioSO goodMusic;
        [SerializeField] private AudioSO badMusic;
        [SerializeField] private AudioSO transitionAudio;
        // [SerializeField] private AudioSO breakAudio;

        [SerializeField] private bool playMusicOnAwake;

        [SerializeField] private float musicTransitionDuration;

        [SerializeField] private BoolEventListener onGamePausedListener;

        private bool _isBroken;
        private float _currentTime;
        private MusicMachineBreaksEvent _machineBreaksEvent;
        private MusicRequest _musicRequest;

        private Tweener _tweener;

        private void Awake()
        {
            _machineBreaksEvent = GetComponent<MusicMachineBreaksEvent>();
            _musicRequest = GetComponent<MusicRequest>();
            _isBroken = false;

            if (playMusicOnAwake)
            {
                goodMusic.Play(fadeIn: true, fadeDuration: 0.2f);
            }
        }

        private void OnEnable()
        {
            onGamePausedListener.Response.AddListener(OnGamePaused);

            ElectricalBox.OnPowerOutage += OnPowerOutage;
            ElectricalBox.OnPowerFixed += OnPowerFixed;
            _musicRequest.OnRequestCompleted += OnMusicRequestCompleted;

            DoomsdayTimer.DoomsdayArrived += DoomsdayArrived;
        }

        private void OnDisable()
        {
            onGamePausedListener.Response.RemoveListener(OnGamePaused);

            ElectricalBox.OnPowerOutage -= OnPowerOutage;
            ElectricalBox.OnPowerFixed -= OnPowerFixed;
            _musicRequest.OnRequestCompleted -= OnMusicRequestCompleted;

            DoomsdayTimer.DoomsdayArrived -= DoomsdayArrived;
        }

        private void DoomsdayArrived()
        {
            if (_isBroken)
            {
                badMusic.Stop(true, 5.0f);
            }
            else
            {
                goodMusic.Stop(true, 5.0f);
            }
        }

        public void StartMusic()
        {
            goodMusic.Play(fadeIn: true, fadeDuration: 2.0f);
        }

        private void OnMusicRequestCompleted()
        {
            // breakAudio.Play();
            goodMusic.CrossFadeAudio(transitionAudio, musicTransitionDuration);
            _tweener.Kill(true);
            if (_isBroken)
            {
                _isBroken = false;
                // breakAudio.Play();
                // goodMusic.CrossFadeAudio(transitionAudio, musicTransitionDuration);
                // _tweener.Kill(true);
            }
        }

        private void Update()
        {
            if (_isBroken || !ElectricalBox.IsPowerOn())
            {
                return;
            }

            _currentTime += Time.deltaTime;
            if (_currentTime > machineBreakCheckFrequency)
            {
                _currentTime = 0.0f;
                if (Random.Range(0.0f, 1.0f) < machineBreakChance)
                {
                    _currentTime = -machineBreakCooldown;
                    BreakMachine();
                }
            }
        }

        private void BreakMachine()
        {
            _isBroken = true;
            _machineBreaksEvent.TriggerEvent();
            _tweener = transform.DOShakeRotation(5.0f, new Vector3(0.0f, 0.0f, 5.0f), 20, 5, false,
                    ShakeRandomnessMode.Harmonic)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);

            // breakAudio.Play();
            badMusic.CrossFadeAudio(transitionAudio, musicTransitionDuration);
        }

        private void OnPowerOutage()
        {
            if (_isBroken)
            {
                badMusic.Stop(true, 0.2f);
            }
            else
            {
                goodMusic.Stop(true, 0.2f);
            }
        }

        private void OnPowerFixed()
        {
            if (_isBroken)
            {
                badMusic.Play(fadeIn: true, fadeDuration: 0.2f);
            }
            else
            {
                goodMusic.Play(fadeIn: true, fadeDuration: 0.2f);
            }
        }

        private void OnGamePaused(bool isPaused)
        {
            if (isPaused)
            {
                if (_isBroken)
                {
                    badMusic.FadeAudio(0.1f, 1.0f);
                }
                else
                {
                    goodMusic.FadeAudio(0.1f, 1.0f);
                }
            }
            else
            {
                if (_isBroken)
                {
                    badMusic.UnFadeAudio(1.0f);
                }
                else
                {
                    goodMusic.UnFadeAudio(1.0f);
                }
            }
        }
    }
}