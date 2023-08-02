using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelinePlayables.FocusSprite
{
    [Serializable]
    public class SpriteFocusData
    {
        public ExposedReference<SpriteRenderer> spriteRendererReference;
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [HideInInspector] public bool isInitialized;

        [HideInInspector] public float startingZPosition;

        [HideInInspector] public int startingSortLayer;
        [HideInInspector] public int startingSortOrder;
    }

    [Serializable]
    public class FocusSpriteClip : PlayableAsset, ITimelineClipAsset
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

                if (spriteData.spriteRenderer == null || spriteData.isInitialized)
                {
                    continue;
                }

                spriteData.startingZPosition = spriteData.spriteRenderer.transform.position.z;

                spriteData.startingSortLayer = spriteData.spriteRenderer.sortingLayerID;
                spriteData.startingSortOrder = spriteData.spriteRenderer.sortingOrder;
                spriteData.isInitialized = true;
            }

            clone.spriteDatas = spriteDatas;
            clone.canClickTargets = canClickTargets;
            clone.fadedFocusCanvas = fadedFocusCanvas.Resolve(graph.GetResolver());
            return playable;
        }
    }
}