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
        private readonly List<Vector3> _iconPositions = new List<Vector3>();
        private readonly List<Vector3> _availableIconPositions = new List<Vector3>();
        private readonly List<NeedsIconData> _currentlyActiveIcons = new List<NeedsIconData>();

        private void Awake()
        {
            unresolvedRequestSprite.enabled = false;

            foreach (NeedsIconData needIcon in _needsIconDatas)
            {
                needIcon.SpriteRenderer.enabled = false;
                _iconPositions.Add(needIcon.SpriteRenderer.transform.localPosition);
            }

            _availableIconPositions.AddRange(_iconPositions);
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

                    if (_availableIconPositions.Count > 0)
                    {
                        iconData.SpriteRenderer.transform.localPosition = _availableIconPositions[^1];
                        _availableIconPositions.RemoveAt(_availableIconPositions.Count - 1);
                    }
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

                _availableIconPositions.Add(iconData.SpriteRenderer.transform.localPosition);
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