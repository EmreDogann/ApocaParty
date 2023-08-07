using System;
using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline.ChangeSortOrder
{
    [Serializable]
    public class ExposedSpriteRenderer : ExposedReferenceHolder<SpriteRenderer> {}

    [Serializable]
    public class SpriteSortData
    {
        public ExposedSpriteRenderer spriteRendererReference;
        [HideInInspector] public SpriteRenderer spriteRenderer;
        [Separator("Initial Sorting Layer")]
        public bool overrideInitialSorting;
        [ConditionalField(nameof(overrideInitialSorting))] [SpriteLayer] public int initSortingLayer;
        [ConditionalField(nameof(overrideInitialSorting))] public int initSortingOrder;

        [HideInInspector] public int startingSortLayer;
        [HideInInspector] public int startingSortOrder;
    }

    [Serializable]
    public class ChangeSortOrderClip : PlayableAsset, ITimelineClipAsset
    {
        [HideInInspector] public ChangeSortOrderBehaviour template = new ChangeSortOrderBehaviour();

        [Separator("Target Sorting Layer")]
        [SpriteLayer] [SerializeField] private int targetSortingLayer;
        [SerializeField] private int targetSortingOrder;
        [SerializeField] private bool resetToInitialOnFinished;

        [Space]
        [SerializeField] private List<SpriteSortData> spriteDatas = new List<SpriteSortData>();

        public ClipCaps clipCaps => ClipCaps.ClipIn;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ChangeSortOrderBehaviour>.Create(graph, template);
            ChangeSortOrderBehaviour clone = playable.GetBehaviour();
            foreach (SpriteSortData spriteData in spriteDatas)
            {
                spriteData.spriteRenderer =
                    spriteData.spriteRendererReference.exposedReference.Resolve(graph.GetResolver());
            }

            clone.spriteDatas = spriteDatas;
            clone.targetSortingLayer = targetSortingLayer;
            clone.targetSortingOrder = targetSortingOrder;
            clone.resetToInitialOnFinished = resetToInitialOnFinished;

            return playable;
        }
    }
}