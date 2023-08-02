using System;
using Audio;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor.Timeline;
#endif

namespace TimelinePlayables.TextPopup
{
    [Serializable]
    public class TextPopupBehaviour : PlayableBehaviour
    {
        [HideInInspector] public TextMeshProUGUI textUI;
        [HideInInspector] public Vector3 startingScale;
        [HideInInspector] public Vector3 position;

        [HideInInspector] public float popupAnimationDuration;
        [HideInInspector] public string text;
        [HideInInspector] public TextPopupShowType showType;
        [HideInInspector] public float nudgeScaleMultiplier;
        [HideInInspector] public int nudgeStrength;

        [HideInInspector] public bool hideWhenFinished;
        [HideInInspector] public AudioSO popupShowAudio;
        [HideInInspector] public AudioSO popupHideAudio;

        private PlayableGraph _graph;
        private Playable _thisPlayable;

        private bool _shown;
        private bool _began;

        public override void OnPlayableCreate(Playable playable)
        {
            _graph = playable.GetGraph();
            _thisPlayable = playable;
            _shown = false;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (textUI == null || _began)
            {
                return;
            }

            _began = true;
            textUI.DOKill();

#if UNITY_EDITOR
            // Check if timeline is being scrubbed/played in edit mode.
            // From: https://forum.unity.com/threads/how-do-you-detect-by-script-when-a-timeline-is-being-played.1000279/
            if (Application.isPlaying == false && TimelineEditor.inspectedDirector != null)
            {
                _shown = true;
                if (popupShowAudio)
                {
                    popupShowAudio.PlayPreview();
                }

                return;
            }

            Show(showType);
#else
            Show(popupType);
#endif

            if (popupShowAudio)
            {
                popupShowAudio.Play2D();
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (textUI == null)
            {
                return;
            }

            double duration = playable.GetDuration();
            double count = playable.GetTime() + info.deltaTime;

            if (info.effectivePlayState != PlayState.Paused && !playable.GetGraph().GetRootPlayable(0).IsDone())
            {
                return;
            }

            _began = false;

            // Checks if reached the end of the clip
            if (count >= duration && hideWhenFinished)
            {
                ResetData();
            }
            else if (count < duration)
            {
                _shown = false;
                textUI.transform.localScale = startingScale;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (textUI == null)
            {
                return;
            }

            _began = false;
            _shown = false;
            textUI.DOKill();
            textUI.transform.localScale = startingScale;
        }

        public bool IsShown()
        {
            return _shown;
        }

        private void ResetData()
        {
            textUI.DOKill();

#if UNITY_EDITOR
            if (Application.isPlaying == false && TimelineEditor.inspectedDirector != null)
            {
                _shown = false;
                if (popupHideAudio)
                {
                    popupHideAudio.PlayPreview();
                }

                return;
            }

            Hide();
#else
            Hide();
#endif

            if (popupHideAudio)
            {
                popupHideAudio.Play2D();
            }
        }

        private void Show(TextPopupShowType popupType)
        {
            switch (popupType)
            {
                case TextPopupShowType.Popup:
                    textUI.transform.DOScale(startingScale, popupAnimationDuration)
                        .From(Vector3.zero)
                        .SetEase(Ease.OutBack)
                        .OnStart(() => _shown = true);
                    break;
                case TextPopupShowType.Nudge:
                    textUI.transform.DOPunchScale(startingScale * nudgeScaleMultiplier - startingScale,
                            popupAnimationDuration,
                            nudgeStrength, 0.0f)
                        .SetEase(Ease.OutBack)
                        .OnStart(() => _shown = true);
                    break;
            }
        }

        private void Hide()
        {
            textUI.transform.DOScale(0.0f, popupAnimationDuration)
                .From(startingScale)
                .SetEase(Ease.InBack)
                .OnComplete(() => _shown = false);
        }
    }
}