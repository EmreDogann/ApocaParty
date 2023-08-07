using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Playables;

namespace Timeline.ToggleInteractable
{
    [Serializable]
    public class ToggleInteractableBehaviour : PlayableBehaviour
    {
        public List<InteractableData> interactableDatas;

        private PlayableGraph _graph;
        private Playable _thisPlayable;

        public override void OnPlayableCreate(Playable playable)
        {
            _graph = playable.GetGraph();
            _thisPlayable = playable;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            foreach (InteractableData interactableData in interactableDatas.Where(interactableData =>
                         interactableData.collider2D != null))
            {
                interactableData.collider2D.enabled = interactableData.activeState;
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
            foreach (InteractableData interactableData in interactableDatas.Where(interactableData =>
                         interactableData.collider2D != null))
            {
                interactableData.collider2D.enabled = interactableData.initialActiveState;
            }
        }
    }
}