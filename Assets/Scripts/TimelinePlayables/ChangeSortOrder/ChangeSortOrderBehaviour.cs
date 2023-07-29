using System;
using MyBox;
using UnityEngine;
using UnityEngine.Playables;

namespace TimelinePlayables.ChangeSortOrder
{
    [Serializable]
    public class ChangeSortOrderBehaviour : PlayableBehaviour
    {
        public SpriteRenderer spriteRenderer;
        [Separator("Initial Sorting Layer")]
        public bool overrideInitialSorting;
        [ConditionalField(nameof(overrideInitialSorting))] [SpriteLayer] [SerializeField] private int initSortingLayer;
        [ConditionalField(nameof(overrideInitialSorting))] [SerializeField] private int initSortingOrder;

        [Separator("Target Sorting Layer")]
        [SpriteLayer] [SerializeField] private int targetSortingLayer;
        [SerializeField] private int targetSortingOrder;
        public bool resetToInitialOnFinished;

        private PlayableGraph _graph;
        private Playable _thisPlayable;
        private bool _began;

        private int _startingSortLayer;
        private int _startingSortOrder;

        public override void OnPlayableCreate(Playable playable)
        {
            _graph = playable.GetGraph();
            _thisPlayable = playable;
            _began = false;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!_began)
            {
                _began = true;
                _startingSortLayer = spriteRenderer.sortingLayerID;
                _startingSortOrder = spriteRenderer.sortingOrder;
            }

            spriteRenderer.sortingLayerID = targetSortingLayer;
            spriteRenderer.sortingOrder = targetSortingOrder;
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            // double duration = playable.GetDuration();
            // double count = playable.GetTime() + info.deltaTime;
            // double startCount = playable.time

            // Checks if reached the end of the clip
            // if (info.effectivePlayState == PlayState.Paused && count > duration ||
            //     playable.GetGraph().GetRootPlayable(0).IsDone())
            if (info.effectivePlayState == PlayState.Paused ||
                playable.GetGraph().GetRootPlayable(0).IsDone())
            {
                if (resetToInitialOnFinished)
                {
                    spriteRenderer.sortingLayerID = overrideInitialSorting ? initSortingLayer : _startingSortLayer;
                    spriteRenderer.sortingOrder = overrideInitialSorting ? initSortingOrder : _startingSortOrder;
                }
            }
        }
    }
}