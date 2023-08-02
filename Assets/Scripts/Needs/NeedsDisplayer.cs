using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace Needs
{
    public class NeedsDisplayer : MonoBehaviour
    {
        [Serializable]
        private class NeedsIconData
        {
            public NeedType NeedType;
            public SpriteRenderer SpriteRenderer;
            [HideInInspector] public bool needResolved;
        }

        [SerializeField] private bool useUnresolvedSymbol;
        [ConditionalField(nameof(useUnresolvedSymbol))] [SerializeField] private SpriteRenderer unresolvedRequestSprite;
        [SerializeField] private List<NeedsIconData> _needsIconDatas;
        private List<NeedsIconData> _currentlyActiveIcons;


        private void Awake()
        {
            unresolvedRequestSprite.enabled = false;

            _currentlyActiveIcons = new List<NeedsIconData>();
            foreach (NeedsIconData needIcon in _needsIconDatas)
            {
                needIcon.SpriteRenderer.enabled = false;
            }
        }

        public void AddDisplay(NeedType needType)
        {
            NeedsIconData iconData = _needsIconDatas.FirstOrDefault(x => x.NeedType == needType);
            if (iconData != null)
            {
                if (useUnresolvedSymbol)
                {
                    unresolvedRequestSprite.enabled = true;
                    iconData.SpriteRenderer.enabled = false;
                    iconData.needResolved = false;
                }
                else
                {
                    iconData.SpriteRenderer.enabled = true;
                    iconData.needResolved = true;
                }

                _currentlyActiveIcons.Add(iconData);
            }
        }

        public void RemoveDisplay(NeedType needType)
        {
            NeedsIconData iconData = _currentlyActiveIcons.Find(x => x.NeedType == needType);
            if (iconData != null)
            {
                iconData.SpriteRenderer.enabled = false;
                iconData.needResolved = false;
                _currentlyActiveIcons.Remove(iconData);
            }
        }

        public bool IsNeedResolved(NeedType needType)
        {
            NeedsIconData iconData = _currentlyActiveIcons.Find(x => x.NeedType == needType);
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

            unresolvedRequestSprite.enabled = false;

            foreach (NeedsIconData activeIcon in _currentlyActiveIcons)
            {
                if (!activeIcon.needResolved)
                {
                    activeIcon.needResolved = true;
                    activeIcon.SpriteRenderer.enabled = true;
                }
            }
        }
    }
}