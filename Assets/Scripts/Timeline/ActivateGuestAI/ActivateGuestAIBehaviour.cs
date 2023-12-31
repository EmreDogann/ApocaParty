using System;
using Guest;
using UnityEngine;
using UnityEngine.Playables;

namespace Timeline.ActivateGuestAI
{
    [Serializable]
    public class ActivateGuestAIBehaviour : PlayableBehaviour
    {
        [HideInInspector] public GuestAI guestAI;

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
                guestAI.ActivateAI();
            }
        }
    }
}