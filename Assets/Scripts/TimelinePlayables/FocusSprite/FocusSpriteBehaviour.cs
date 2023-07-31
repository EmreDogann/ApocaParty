using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

namespace TimelinePlayables.FocusSprite
{
    [Serializable]
    public class FocusSpriteBehaviour : PlayableBehaviour
    {
        [HideInInspector] public List<SpriteFocusData> spriteDatas;
        [HideInInspector] public Canvas fadedFocusCanvas;

        [HideInInspector] public bool canClickTargets;
        [HideInInspector] public bool enableChangingSorting;

        [HideInInspector] public bool resetPositionOnFinished;
        [HideInInspector] public bool resetSortingOnFinished;

        private PlayableGraph _graph;
        private Playable _thisPlayable;

        public override void OnPlayableCreate(Playable playable)
        {
            _graph = playable.GetGraph();
            _thisPlayable = playable;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (fadedFocusCanvas == null)
            {
                return;
            }

            foreach (SpriteFocusData spriteData in spriteDatas.Where(spriteData => spriteData.spriteRenderer != null))
            {
                if (canClickTargets)
                {
                    spriteData.spriteRenderer.transform.position = new Vector3(spriteData.startingPosition.x,
                        spriteData.startingPosition.y, fadedFocusCanvas.transform.position.z * 1.1f);
                }

                if (enableChangingSorting)
                {
                    spriteData.spriteRenderer.sortingLayerID = fadedFocusCanvas.sortingLayerID;
                    spriteData.spriteRenderer.sortingOrder = fadedFocusCanvas.sortingOrder;
                }
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (info.effectivePlayState == PlayState.Paused || playable.GetGraph().GetRootPlayable(0).IsDone())
            {
                ResetData();
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            ResetData();
        }

        private void ResetData()
        {
            if (!canClickTargets && !enableChangingSorting ||
                !resetPositionOnFinished && !resetSortingOnFinished)
            {
                return;
            }

            foreach (SpriteFocusData spriteData in spriteDatas.Where(spriteData => spriteData.spriteRenderer != null))
            {
                if (canClickTargets && resetPositionOnFinished)
                {
                    spriteData.spriteRenderer.transform.position = spriteData.startingPosition;
                }

                if (enableChangingSorting && resetSortingOnFinished)
                {
                    spriteData.spriteRenderer.sortingLayerID = spriteData.startingSortLayer;
                    spriteData.spriteRenderer.sortingOrder = spriteData.startingSortLayer;
                }
            }
        }
    }
}