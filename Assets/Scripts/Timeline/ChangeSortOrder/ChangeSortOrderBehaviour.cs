using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Timeline.ChangeSortOrder
{
    [Serializable]
    public class ChangeSortOrderBehaviour : PlayableBehaviour
    {
        [HideInInspector] public List<SpriteSortData> spriteDatas;

        [HideInInspector] public int targetSortingLayer;
        [HideInInspector] public int targetSortingOrder;
        [HideInInspector] public bool resetToInitialOnFinished;

        private PlayableGraph _graph;
        private Playable _thisPlayable;
        private bool _began;

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
                foreach (SpriteSortData spriteData in spriteDatas)
                {
                    if (spriteData.spriteRenderer == null)
                    {
                        continue;
                    }

                    spriteData.startingSortLayer = spriteData.spriteRenderer.sortingLayerID;
                    spriteData.startingSortOrder = spriteData.spriteRenderer.sortingOrder;
                }
            }

            foreach (SpriteSortData spriteData in spriteDatas)
            {
                if (spriteData.spriteRenderer == null)
                {
                    continue;
                }

                spriteData.spriteRenderer.sortingLayerID = targetSortingLayer;
                spriteData.spriteRenderer.sortingOrder = targetSortingOrder;
            }
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
                if (!resetToInitialOnFinished)
                {
                    return;
                }

                foreach (SpriteSortData spriteData in spriteDatas)
                {
                    if (spriteData.spriteRenderer == null)
                    {
                        continue;
                    }

                    spriteData.spriteRenderer.sortingLayerID = spriteData.overrideInitialSorting
                        ? spriteData.initSortingLayer
                        : spriteData.startingSortLayer;
                    spriteData.spriteRenderer.sortingOrder = spriteData.overrideInitialSorting
                        ? spriteData.initSortingLayer
                        : spriteData.startingSortLayer;
                }
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);

            foreach (SpriteSortData spriteData in spriteDatas)
            {
                if (spriteData.spriteRenderer == null)
                {
                    continue;
                }

                spriteData.spriteRenderer.sortingLayerID = spriteData.overrideInitialSorting
                    ? spriteData.initSortingLayer
                    : spriteData.startingSortLayer;
                spriteData.spriteRenderer.sortingOrder = spriteData.overrideInitialSorting
                    ? spriteData.initSortingLayer
                    : spriteData.startingSortLayer;
            }
        }
    }
}