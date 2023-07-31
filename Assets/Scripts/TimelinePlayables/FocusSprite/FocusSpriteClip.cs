using System;
using System.Collections.Generic;
using MyBox;
using TimelinePlayables.ChangeSortOrder;
using TimelinePlayables.FocusSprite;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelinePlayables.FocusSprite
{
    [Serializable]
    public class SpriteFocusData
    {
        public ExposedSpriteRenderer spriteRendererReference;
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [HideInInspector] public bool isInitialized;

        [HideInInspector] public Vector3 startingPosition;

        [HideInInspector] public int startingSortLayer;
        [HideInInspector] public int startingSortOrder;
    }
}

[Serializable]
public class FocusSpriteClip : PlayableAsset, ITimelineClipAsset
{
    [HideInInspector] public FocusSpriteBehaviour template = new FocusSpriteBehaviour();

    [SerializeField] private bool canClickTargets;
    [ConditionalField(nameof(canClickTargets))] [SerializeField] private bool resetPositionOnFinished;
    public ExposedReference<Canvas> fadedFocusCanvas;

    [Space]
    [SerializeField] private bool enableChangingSorting;
    [ConditionalField(nameof(enableChangingSorting))] [SerializeField] private bool resetSortingOnFinished;

    [SerializeField] private List<SpriteFocusData> spriteDatas = new List<SpriteFocusData>();

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<FocusSpriteBehaviour>.Create(graph, template);
        FocusSpriteBehaviour clone = playable.GetBehaviour();
        foreach (SpriteFocusData spriteData in spriteDatas)
        {
            spriteData.spriteRenderer =
                spriteData.spriteRendererReference.exposedReference.Resolve(graph.GetResolver());

            if (spriteData.spriteRenderer == null || spriteData.isInitialized)
            {
                continue;
            }

            spriteData.startingPosition = spriteData.spriteRenderer.transform.position;

            spriteData.startingSortLayer = spriteData.spriteRenderer.sortingLayerID;
            spriteData.startingSortOrder = spriteData.spriteRenderer.sortingOrder;
            spriteData.isInitialized = true;
        }

        clone.spriteDatas = spriteDatas;
        clone.canClickTargets = canClickTargets;
        clone.fadedFocusCanvas = fadedFocusCanvas.Resolve(graph.GetResolver());
        clone.resetPositionOnFinished = resetPositionOnFinished;

        clone.enableChangingSorting = enableChangingSorting;
        clone.resetSortingOnFinished = resetSortingOnFinished;
        return playable;
    }
}