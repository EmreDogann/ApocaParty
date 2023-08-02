using System;
using Audio;
using MyBox;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelinePlayables.TextPopup
{
    public enum TextPopupShowType
    {
        Popup,
        Nudge
    }

    [Serializable]
    public class TextPopupClip : PlayableAsset, ITimelineClipAsset
    {
        [HideInInspector] public TextPopupBehaviour template = new TextPopupBehaviour();
        [TextArea(3, 5)]
        public string text;
        public ExposedReference<Transform> targetPosition;

        [Header("Animation")]
        public float popupAnimationDuration;
        public TextPopupShowType showType;
        [ConditionalField(nameof(showType), false, TextPopupShowType.Nudge)]
        [Range(0.0f, 5.0f)] public float nudgeScaleMultiplier = 1;
        [ConditionalField(nameof(showType), false, TextPopupShowType.Nudge)] [Range(0, 10)] public int nudgeStrength;

        public bool hideWhenFinished;

        [Space]
        [Header("Audio")]
        public AudioSO popupShowAudio;
        public AudioSO popupHideAudio;

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TextPopupBehaviour>.Create(graph, template);
            TextPopupBehaviour clone = playable.GetBehaviour();
            clone.text = text;

            Transform transform = targetPosition.Resolve(graph.GetResolver());
            if (transform)
            {
                clone.position = transform.position;
            }

            clone.nudgeScaleMultiplier = nudgeScaleMultiplier;
            clone.nudgeStrength = nudgeStrength;
            clone.popupAnimationDuration = popupAnimationDuration;
            clone.popupShowAudio = popupShowAudio;
            clone.popupHideAudio = popupHideAudio;
            clone.hideWhenFinished = hideWhenFinished;
            clone.showType = showType;

            return playable;
        }
    }
}