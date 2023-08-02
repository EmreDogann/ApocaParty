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
        public List<SpriteFocusData> spriteDatas;
        public Canvas fadedFocusCanvas;

        public bool canClickTargets;

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
                    Transform transform = spriteData.spriteRenderer.transform;
                    transform.position = new Vector3(transform.position.x,
                        transform.position.y, fadedFocusCanvas.transform.position.z * 1.1f);
                }

                spriteData.spriteRenderer.sortingLayerID = fadedFocusCanvas.sortingLayerID;
                spriteData.spriteRenderer.sortingOrder = fadedFocusCanvas.sortingOrder + 1;
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
            foreach (SpriteFocusData spriteData in spriteDatas.Where(spriteData => spriteData.spriteRenderer != null))
            {
                Vector3 transformPosition = spriteData.spriteRenderer.transform.position;
                transformPosition.z = spriteData.startingZPosition;

                spriteData.spriteRenderer.transform.position = transformPosition;

                spriteData.spriteRenderer.sortingLayerID = spriteData.startingSortLayer;
                spriteData.spriteRenderer.sortingOrder = spriteData.startingSortOrder;
            }
        }
    }
}