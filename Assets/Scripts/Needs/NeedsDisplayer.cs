﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using DG.Tweening;
using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
            [HideInInspector] public Sequence popupEffect;

            public Vector2 JumpEndvalue => containerRectTransform.anchoredPosition;
        }

        [SerializeField] private bool useUnresolvedSymbol;
        [ConditionalField(nameof(useUnresolvedSymbol))] [SerializeField] private Image unresolvedRequestImage;
        [SerializeField] private List<NeedsIconData> needsIconDatas;

        [SerializeField] private AudioSO needPopupSound;
        [SerializeField] private AudioSO unknownRequestSound;

        private readonly List<NeedsIconData> _currentlyActiveIcons = new List<NeedsIconData>();

        private Sequence _unknownRequestTween;
        private Sequence _iconTween;

        private void Awake()
        {
            if (useUnresolvedSymbol)
            {
                unresolvedRequestImage.gameObject.SetActive(false);

                _unknownRequestTween = DOTween.Sequence();
                _unknownRequestTween
                    .AppendInterval(0.85f)
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
                    .Pause();
            }

            foreach (NeedsIconData needIcon in needsIconDatas)
            {
                needIcon.ID = GUID.Generate().GetHashCode();
                needIcon.canvasGroup = needIcon.icon.rectTransform.parent.GetComponent<CanvasGroup>();

                needIcon.containerRectTransform = needIcon.icon.rectTransform.parent as RectTransform;
                needIcon.containerRectTransform.parent.gameObject.SetActive(false);

                needIcon.popupEffect = DOTween.Sequence();
                needIcon.popupEffect
                    .SetId(needIcon.ID)
                    .Append(needIcon.containerRectTransform
                        .DOScale(needIcon.containerRectTransform.localScale * 1.2f, 0.5f)
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

                        needPopupSound.Play(transform.position);
                    })
                    .OnRewind(() =>
                    {
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

        // Hack to fix FlowLayoutGroup.cs not updating with the latest state.
        private IEnumerator DelayedJumpTween(NeedsIconData needIcon)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(needIcon.containerRectTransform.parent.parent as RectTransform);
            yield return null;
            needIcon.containerRectTransform
                .DOJumpAnchorPos(needIcon.containerRectTransform.anchoredPosition, 75.0f, 1, 0.8f)
                .SetEase(Ease.OutBounce);
        }

        public void UpdateProgress(NeedType needType, float progressPercentage)
        {
            NeedsIconData iconData = _currentlyActiveIcons.FirstOrDefault(x => x.needType == needType);
            if (iconData != null)
            {
                iconData.icon.fillAmount = Mathf.Clamp01(progressPercentage);
            }
        }

        public void AddDisplay(NeedType needType)
        {
            NeedsIconData iconData = needsIconDatas.FirstOrDefault(x => x.needType == needType);
            if (iconData != null)
            {
                if (useUnresolvedSymbol)
                {
                    iconData.needResolved = false;
                    _unknownRequestTween.PlayForward();
                }
                else
                {
                    iconData.needResolved = true;
                    iconData.popupEffect.PlayForward();
                }

                _currentlyActiveIcons.Add(iconData);
            }
        }

        public void RemoveDisplay(NeedType needType)
        {
            NeedsIconData iconData = _currentlyActiveIcons.Find(x => x.needType == needType);
            if (iconData != null)
            {
                iconData.popupEffect.Rewind();
                iconData.needResolved = false;
                _currentlyActiveIcons.Remove(iconData);

                if (useUnresolvedSymbol && _currentlyActiveIcons.Count == 0)
                {
                    _unknownRequestTween.Rewind();
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

        public void ResolveNeed()
        {
            if (!useUnresolvedSymbol)
            {
                return;
            }

            _unknownRequestTween.Rewind();
            foreach (NeedsIconData activeIcon in _currentlyActiveIcons)
            {
                if (!activeIcon.needResolved)
                {
                    activeIcon.needResolved = true;
                    activeIcon.popupEffect.PlayForward();
                }
            }
        }
    }
}