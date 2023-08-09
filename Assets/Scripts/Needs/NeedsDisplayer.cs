using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using DG.Tweening;
using MyBox;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Needs
{
    public class NeedsDisplayer : MonoBehaviour
    {
        [Serializable]
        private class NeedsIconData
        {
            [HideInInspector] public int ID;
            public NeedType needType;
            public Image icon;
            [HideInInspector] public CanvasGroup canvasGroup;
            [HideInInspector] public bool needResolved;
            [HideInInspector] public RectTransform containerRectTransform;
            [HideInInspector] public Sequence PopupEffect;
            [HideInInspector] public Sequence ShakeEffect;
        }

        [SerializeField] private bool useUnresolvedSymbol;
        [ConditionalField(nameof(useUnresolvedSymbol))] [SerializeField] private Image unresolvedRequestImage;
        [SerializeField] private List<NeedsIconData> needsIconDatas;

        [SerializeField] private AudioSO needPopupSound;
        [SerializeField] private AudioSO unknownRequestSound;

        private readonly List<NeedsIconData> _currentlyActiveIcons = new List<NeedsIconData>();

        private Sequence _unknownRequestTween;
        private Sequence _iconTween;

        private Coroutine playDelayedTweenCoroutine;

        private void Awake()
        {
            if (useUnresolvedSymbol)
            {
                unresolvedRequestImage.gameObject.SetActive(false);

                _unknownRequestTween = DOTween.Sequence();
                _unknownRequestTween
                    .Append(unresolvedRequestImage.rectTransform
                        .DOJumpAnchorPos(unresolvedRequestImage.rectTransform.anchoredPosition, 75.0f, 1, 0.8f)
                        .SetEase(Ease.OutBounce))
                    .SetLoops(-1, LoopType.Restart)
                    .OnPlay(() =>
                    {
                        Color color = unresolvedRequestImage.color;
                        color.a = 0.0f;
                        unresolvedRequestImage.color = color;

                        unresolvedRequestImage.gameObject.SetActive(true);
                        unresolvedRequestImage.DOFade(1.0f, 0.2f);

                        unknownRequestSound.Play(transform.position);
                    })
                    .OnRewind(() =>
                    {
                        unresolvedRequestImage.DOFade(0.0f, 0.2f).OnComplete(() =>
                        {
                            unresolvedRequestImage.gameObject.SetActive(false);
                        });
                    })
                    .SetAutoKill(false)
                    .Pause();
            }

            foreach (NeedsIconData needIcon in needsIconDatas)
            {
                needIcon.ID = Guid.NewGuid().GetHashCode();
                needIcon.canvasGroup = needIcon.icon.rectTransform.parent.GetComponent<CanvasGroup>();

                needIcon.containerRectTransform = needIcon.icon.rectTransform.parent as RectTransform;
                needIcon.containerRectTransform.parent.gameObject.SetActive(false);

                needIcon.ShakeEffect = DOTween.Sequence();
                needIcon.ShakeEffect
                    .SetId(needIcon.ID)
                    .Append(needIcon.containerRectTransform
                        .DOLocalRotate(needIcon.containerRectTransform.localRotation * Vector3.forward * 1.5f, 0.0f))
                    .AppendInterval(0.75f)
                    .Append(needIcon.containerRectTransform
                        .DOLocalRotate(needIcon.containerRectTransform.localRotation * Vector3.forward * -1.5f, 0.0f))
                    .SetLoops(-1, LoopType.Yoyo)
                    .Pause();

                needIcon.PopupEffect = DOTween.Sequence();
                needIcon.PopupEffect
                    .SetId(needIcon.ID)
                    .Append(needIcon.containerRectTransform
                        .DOScale(needIcon.containerRectTransform.localScale * 1.5f, 0.5f)
                        .SetEase(Ease.OutQuad)
                        .From())
                    .OnPlay(() =>
                    {
                        needIcon.canvasGroup.alpha = 0.0f;

                        needIcon.containerRectTransform.parent.gameObject.SetActive(true);
                        if (needIcon.canvasGroup)
                        {
                            needIcon.canvasGroup.DOFade(1.0f, 0.2f);
                        }

                        StartCoroutine(DelayedJumpTween(needIcon));
                        needIcon.ShakeEffect
                            .SetDelay(Random.Range(0.0f, 2.0f), false)
                            .PlayForward();

                        needPopupSound.Play(transform.position);
                    })
                    .OnRewind(() =>
                    {
                        needIcon.ShakeEffect.Rewind();
                        if (needIcon.canvasGroup)
                        {
                            needIcon.canvasGroup.DOFade(0.0f, 0.2f).OnComplete(() =>
                            {
                                needIcon.containerRectTransform.parent.gameObject.SetActive(false);
                            });
                        }
                    })
                    .SetAutoKill(false)
                    .Pause();
            }
        }

        private void OnDestroy()
        {
            _unknownRequestTween.Kill();

            foreach (NeedsIconData needIcon in needsIconDatas)
            {
                needIcon.PopupEffect.Kill();
                needIcon.ShakeEffect.Kill();
            }
        }

        // Hack to fix FlowLayoutGroup.cs not updating with the latest state.
        private IEnumerator DelayedJumpTween(NeedsIconData needIcon)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(needIcon.containerRectTransform.parent.parent as RectTransform);
            yield return null;
            needIcon.containerRectTransform
                .DOJumpAnchorPos(needIcon.containerRectTransform.anchoredPosition, 75.0f, 1, 0.8f)
                .SetEase(Ease.OutBounce);
        }

        private IEnumerator DelayedUnknownIconPlay()
        {
            yield return null;
            _unknownRequestTween.PlayForward();
        }

        private void PlayUnknownRequestTween()
        {
            if (playDelayedTweenCoroutine != null)
            {
                StopCoroutine(playDelayedTweenCoroutine);
            }

            playDelayedTweenCoroutine = StartCoroutine(DelayedUnknownIconPlay());
        }

        private void StopUnknownRequestTween()
        {
            if (playDelayedTweenCoroutine != null)
            {
                StopCoroutine(playDelayedTweenCoroutine);
            }

            _unknownRequestTween.Rewind();
        }

        public void UpdateProgress(NeedType needType, float progressPercentage)
        {
            NeedsIconData iconData = _currentlyActiveIcons.FirstOrDefault(x => x.needType == needType);
            if (iconData != null)
            {
                iconData.icon.fillAmount = Mathf.MoveTowards(iconData.icon.fillAmount,
                    Mathf.Clamp01(progressPercentage), 1.0f * Time.deltaTime);
            }
        }

        public bool HasReachedProgress(NeedType needType, float progressPercentage)
        {
            NeedsIconData iconData = _currentlyActiveIcons.FirstOrDefault(x => x.needType == needType);
            if (iconData != null)
            {
                return iconData.icon.fillAmount <= progressPercentage;
            }

            return false;
        }

        public void AddDisplay(NeedType needType)
        {
            NeedsIconData iconData = needsIconDatas.FirstOrDefault(x => x.needType == needType);
            if (iconData != null)
            {
                if (useUnresolvedSymbol)
                {
                    iconData.needResolved = false;
                    PlayUnknownRequestTween();
                }
                else
                {
                    iconData.needResolved = true;
                    iconData.PopupEffect.PlayForward();
                }

                _currentlyActiveIcons.Add(iconData);
            }
        }

        public void AddDisplay(NeedType needType, bool startResolved)
        {
            NeedsIconData iconData = needsIconDatas.FirstOrDefault(x => x.needType == needType);
            if (iconData != null)
            {
                if (useUnresolvedSymbol && !startResolved)
                {
                    iconData.needResolved = false;
                    PlayUnknownRequestTween();
                }
                else
                {
                    iconData.needResolved = true;
                    iconData.PopupEffect.PlayForward();
                }

                _currentlyActiveIcons.Add(iconData);
            }
        }

        public void RemoveDisplay(NeedType needType)
        {
            NeedsIconData iconData = _currentlyActiveIcons.Find(x => x.needType == needType);
            if (iconData != null)
            {
                iconData.PopupEffect.Rewind();
                iconData.needResolved = false;
                _currentlyActiveIcons.Remove(iconData);

                if (useUnresolvedSymbol && _currentlyActiveIcons.Count == 0)
                {
                    StopUnknownRequestTween();
                }
            }
        }

        public bool IsNeedResolved(NeedType needType)
        {
            NeedsIconData iconData = _currentlyActiveIcons.Find(x => x.needType == needType);
            if (iconData != null)
            {
                return iconData.needResolved;
            }

            return false;
        }

        public bool ResolveNeed()
        {
            if (!useUnresolvedSymbol)
            {
                return false;
            }

            StopUnknownRequestTween();
            foreach (NeedsIconData activeIcon in _currentlyActiveIcons)
            {
                if (!activeIcon.needResolved)
                {
                    activeIcon.needResolved = true;
                    activeIcon.PopupEffect.PlayForward();
                }
            }

            return true;
        }

        public bool ResolveNeed(NeedType needType)
        {
            if (!useUnresolvedSymbol)
            {
                return false;
            }

            NeedsIconData iconData = _currentlyActiveIcons.Find(x => x.needType == needType);
            if (iconData != null)
            {
                iconData.needResolved = true;
                iconData.PopupEffect.PlayForward();
            }

            if (_currentlyActiveIcons.Count(x => x.needResolved == false) == 0)
            {
                StopUnknownRequestTween();
            }

            return true;
        }
    }
}