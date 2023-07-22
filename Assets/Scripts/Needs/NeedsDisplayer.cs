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
        }

        [Tooltip("Spacing between the need icons in world space.")]
        [SerializeField] private float iconSpacing = 0.5f;

        [SerializeField] private List<NeedsIconData> _needsIconDatas;
        private List<NeedsIconData> _currentlyActiveIcons;

        private void Awake()
        {
            _currentlyActiveIcons = new List<NeedsIconData>();
            foreach (NeedsIconData needIcons in _needsIconDatas)
            {
                needIcons.SpriteRenderer.enabled = false;
            }
        }

        private void UpdateDisplay()
        {
            float xPosition = 0.0f;
            foreach (NeedsIconData activeIcons in _currentlyActiveIcons)
            {
                Vector3 transformPosition = activeIcons.SpriteRenderer.transform.localPosition;
                transformPosition.x = xPosition;

                activeIcons.SpriteRenderer.transform.localPosition = transformPosition;

                xPosition += iconSpacing;
            }
        }

        public void AddDisplay(NeedType needType)
        {
            NeedsIconData iconData = _needsIconDatas.FirstOrDefault(x => x.NeedType == needType);
            if (iconData != null)
            {
                iconData.SpriteRenderer.enabled = true;
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
                _currentlyActiveIcons.Remove(iconData);

                UpdateDisplay();
            }
        }
    }
}