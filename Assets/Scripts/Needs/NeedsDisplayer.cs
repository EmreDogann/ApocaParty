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
        }

        [SerializeField] private bool useUnresolvedSymbol;
        [ConditionalField(nameof(useUnresolvedSymbol))] [SerializeField] private Image unresolvedRequestImage;
        [SerializeField] private List<NeedsIconData> needsIconDatas;
        private readonly List<NeedsIconData> _currentlyActiveIcons = new List<NeedsIconData>();

        private void Awake()
        {
            if (useUnresolvedSymbol)
            {
                unresolvedRequestImage.enabled = false;
            }

            foreach (NeedsIconData needIcon in needsIconDatas)
            {
                needIcon.icon.enabled = false;
            }
        }

        public void AddDisplay(NeedType needType)
        {
            NeedsIconData iconData = needsIconDatas.FirstOrDefault(x => x.needType == needType);
            if (iconData != null)
            {
                if (useUnresolvedSymbol)
                {
                    unresolvedRequestImage.enabled = true;
                    iconData.icon.enabled = false;
                    iconData.needResolved = false;
                }
                else
                {
                    iconData.icon.enabled = true;
                    iconData.needResolved = true;

                    // iconData.SpriteRenderer.transform.localPosition = _availableIconPositions[^1];
                }

                _currentlyActiveIcons.Add(iconData);
            }
        }

        public void RemoveDisplay(NeedType needType)
        {
            NeedsIconData iconData = _currentlyActiveIcons.Find(x => x.needType == needType);
            if (iconData != null)
            {
                iconData.icon.enabled = false;
                iconData.needResolved = false;
                _currentlyActiveIcons.Remove(iconData);

                if (useUnresolvedSymbol && _currentlyActiveIcons.Count == 0)
                {
                    unresolvedRequestImage.enabled = false;
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

            unresolvedRequestImage.enabled = false;

            foreach (NeedsIconData activeIcon in _currentlyActiveIcons)
            {
                if (!activeIcon.needResolved)
                {
                    activeIcon.needResolved = true;
                    activeIcon.icon.enabled = true;
                }
            }
        }
    }
}