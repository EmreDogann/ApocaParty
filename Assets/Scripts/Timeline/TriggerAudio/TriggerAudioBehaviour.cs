using System;
using Audio;
using MyBox;
using UnityEngine;
using UnityEngine.Playables;

namespace Timeline.TriggerAudio
{
    [Serializable]
    public class TriggerAudioBehaviour : PlayableBehaviour
    {
        public enum AudioTriggerType
        {
            Play,
            Play2D,
            PlayAttached
        }

        public AudioSO audioToTrigger;
        public AudioTriggerType triggerType;
        [ConditionalField(nameof(triggerType), false, AudioTriggerType.Play)] public Transform triggerPosition;

        private PlayableGraph _graph;
        private Playable _thisPlayable;
        private bool _began;

        public override void OnPlayableCreate(Playable playable)
        {
            _graph = playable.GetGraph();
            _thisPlayable = playable;
            _began = false;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!_began)
            {
                _began = true;

                if (!playable.GetGraph().IsPlaying())
                {
                    return;
                }

                switch (triggerType)
                {
                    case AudioTriggerType.Play:
                        audioToTrigger.Play(triggerPosition.position);
                        break;
                    case AudioTriggerType.Play2D:
                        audioToTrigger.Play2D();
                        break;
                }
            }
        }
    }
}