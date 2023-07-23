using System;
using System.Collections.Generic;
using System.Linq;
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
            [HideInInspector] public Sprite requestSprite;
            [HideInInspector] public bool needResolved;
        }

        [Tooltip("Spacing between the need icons in world space.")]
        [SerializeField] private float iconSpacing = 0.5f;

        [SerializeField] private SpriteRenderer unresolvedRequestSprite;
        [SerializeField] private List<NeedsIconData> _needsIconDatas;
        private List<NeedsIconData> _currentlyActiveIcons;

        private void Awake()
        {
            _currentlyActiveIcons = new List<NeedsIconData>();
            foreach (NeedsIconData needIcon in _needsIconDatas)
            {
                needIcon.SpriteRenderer.enabled = false;
                needIcon.requestSprite = needIcon.SpriteRenderer.sprite;
            }
        }

        private void UpdateDisplay()
        {
            float xPosition = 0.0f;
            foreach (NeedsIconData activeIcon in _currentlyActiveIcons)
            {
                Vector3 transformPosition = activeIcon.SpriteRenderer.transform.localPosition;
                transformPosition.x = xPosition;

                activeIcon.SpriteRenderer.transform.localPosition = transformPosition;

                xPosition += iconSpacing;
            }
        }

        public void AddDisplay(NeedType needType)
        {
            NeedsIconData iconData = _needsIconDatas.FirstOrDefault(x => x.NeedType == needType);
            if (iconData != null)
            {
                iconData.SpriteRenderer.sprite = unresolvedRequestSprite.sprite;
                iconData.SpriteRenderer.enabled = true;

                iconData.needResolved = false;
                _currentlyActiveIcons.Add(iconData);

                UpdateDisplay();
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

                UpdateDisplay();
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
            foreach (NeedsIconData activeIcon in _currentlyActiveIcons)
            {
                if (!activeIcon.needResolved)
                {
                    activeIcon.needResolved = true;
                    activeIcon.SpriteRenderer.sprite = activeIcon.requestSprite;
                    activeIcon.SpriteRenderer.enabled = true;
                }
            }
        }
    }
}