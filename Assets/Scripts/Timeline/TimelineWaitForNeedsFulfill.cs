using System.Collections.Generic;
using Needs;
using UnityEngine;
using UnityEngine.Playables;

namespace Timeline
{
    public class TimelineWaitForNeedsFulfill : MonoBehaviour
    {
        [SerializeField] private List<NeedSystem> guestsToWaitFor;

        private bool _waitingForNeedsFulfill;
        private PlayableDirector _currentDirector;

        private void OnEnable()
        {
            foreach (NeedSystem needSystem in guestsToWaitFor)
            {
                needSystem.OnNeedFulfilled += OnNeedFulfilled;
            }
        }

        private void OnDisable()
        {
            foreach (NeedSystem needSystem in guestsToWaitFor)
            {
                needSystem.OnNeedFulfilled -= OnNeedFulfilled;
            }
        }

        private void OnNeedFulfilled(NeedType needType)
        {
            if (!_waitingForNeedsFulfill)
            {
                return;
            }

            foreach (NeedSystem guest in guestsToWaitFor)
            {
                if (guest.HasNeed())
                {
                    return;
                }
            }

            _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
            _currentDirector = null;

            _waitingForNeedsFulfill = false;
        }

        public void WaitForNeedsFulfill(PlayableDirector director)
        {
            if (!_waitingForNeedsFulfill)
            {
                _waitingForNeedsFulfill = true;

                _currentDirector = director;
                director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            }
        }
    }
}