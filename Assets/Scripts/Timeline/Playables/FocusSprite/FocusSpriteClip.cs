using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.Playables.FocusSprite
{
    [Serializable]
    public class SpriteFocusData
    {
        public ExposedReference<SpriteRenderer> spriteRendererReference;
        public ExposedReference<Transform> rootTransformReference;
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [HideInInspector] public Transform rootTransform;
        [HideInInspector] public bool isInitialized;

        [HideInInspector] public float startingZPosition;

        [HideInInspector] public int startingSortLayer;
        [HideInInspector] public int startingSortOrder;
    }

    [Serializable]
    public class FocusSpriteClip : PlayableAsset, IPropertyPreview, ITimelineClipAsset
    {
        [HideInInspector] public FocusSpriteBehaviour template = new FocusSpriteBehaviour();

        [SerializeField] private bool canClickTargets;
        public ExposedReference<Canvas> fadedFocusCanvas;

        [SerializeField] private List<SpriteFocusData> spriteDatas = new List<SpriteFocusData>();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<FocusSpriteBehaviour>.Create(graph, template);
            FocusSpriteBehaviour clone = playable.GetBehaviour();
            foreach (SpriteFocusData spriteData in spriteDatas)
            {
                spriteData.spriteRenderer =
                    spriteData.spriteRendererReference.Resolve(graph.GetResolver());

                if (spriteData.spriteRenderer == null)
                {
                    continue;
                }

                Transform newRootTransform = spriteData.rootTransformReference.Resolve(graph.GetResolver());

                if (newRootTransform == null)
                {
                    spriteData.rootTransform = spriteData.spriteRenderer.transform;
                }
                else if (newRootTransform != spriteData.rootTransform)
                {
                    spriteData.rootTransform = newRootTransform;
                    spriteData.startingZPosition = spriteData.rootTransform.position.z;
                }

                if (spriteData.isInitialized)
                {
                    continue;
                }

                spriteData.startingZPosition = spriteData.rootTransform.position.z;

                spriteData.startingSortLayer = spriteData.spriteRenderer.sortingLayerID;
                spriteData.startingSortOrder = spriteData.spriteRenderer.sortingOrder;
                spriteData.isInitialized = true;
            }

            clone.spriteDatas = spriteDatas;
            clone.canClickTargets = canClickTargets;
            clone.fadedFocusCanvas = fadedFocusCanvas.Resolve(graph.GetResolver());
            return playable;
        }

        public void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            foreach (SpriteFocusData spriteData in spriteDatas)
            {
                if (spriteData.spriteRenderer != null)
                {
                    driver.AddFromName<SpriteRenderer>(spriteData.spriteRenderer.gameObject, "m_SortingLayerID");
                    driver.AddFromName<SpriteRenderer>(spriteData.spriteRenderer.gameObject, "m_SortingOrder");
                }

                if (spriteData.rootTransform != null)
                {
                    driver.AddFromName<Transform>(spriteData.rootTransform.gameObject, "m_LocalPosition.x");
                    driver.AddFromName<Transform>(spriteData.rootTransform.gameObject, "m_LocalPosition.y");
                    driver.AddFromName<Transform>(spriteData.rootTransform.gameObject, "m_LocalPosition.z");
                }
            }
        }
    }
}