using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;
using UnityEngine.UI;

namespace Needs
{
    public class NeedsDisplayer : MonoBehaviour
    {
        [Serializable]
        private class NeedsIconData
        {
            public NeedType needType;
            public Image icon;
            [HideInInspector] public bool needResolved;
            [HideInInspector] public GameObject container;
        }

        [SerializeField] private bool useUnresolvedSymbol;
        [ConditionalField(nameof(useUnresolvedSymbol))] [SerializeField] private Image unresolvedRequestImage;
        [SerializeField] private List<NeedsIconData> needsIconDatas;
        private readonly List<NeedsIconData> _currentlyActiveIcons = new List<NeedsIconData>();

        private void Awake()
        {
            if (useUnresolvedSymbol)
            {
                unresolvedRequestImage.gameObject.SetActive(false);
            }

            foreach (NeedsIconData needIcon in needsIconDatas)
            {
                needIcon.container = needIcon.icon.transform.parent.gameObject;
                needIcon.container.SetActive(false);
            }
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
                    unresolvedRequestImage.gameObject.SetActive(true);
                    iconData.container.SetActive(false);
                    iconData.needResolved = false;
                }
                else
                {
                    iconData.container.SetActive(true);
                    iconData.needResolved = true;
                }

                _currentlyActiveIcons.Add(iconData);
            }
        }

        public void RemoveDisplay(NeedType needType)
        {
            NeedsIconData iconData = _currentlyActiveIcons.Find(x => x.needType == needType);
            if (iconData != null)
            {
                iconData.container.SetActive(false);
                iconData.needResolved = false;
                _currentlyActiveIcons.Remove(iconData);

                if (useUnresolvedSymbol && _currentlyActiveIcons.Count == 0)
                {
                    unresolvedRequestImage.gameObject.SetActive(false);
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

            unresolvedRequestImage.gameObject.SetActive(false);

            foreach (NeedsIconData activeIcon in _currentlyActiveIcons)
            {
                if (!activeIcon.needResolved)
                {
                    activeIcon.needResolved = true;
                    activeIcon.container.SetActive(true);
                }
            }
        }
    }
}