using System.Collections.Generic;
using Needs;
using UnityEngine;
using UnityEngine.Playables;

namespace Timeline
{
    public class TimelineWaitForNeedsFulfill : MonoBehaviour
    {
        [SerializeField] private List<NeedSystem> guestsToWaitFor;
        [SerializeField] private NeedType targetNeed;

        private int _needsFulfilledCount;
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
                needSystem.OnNeedFulfilled += OnNeedFulfilled;
            }
        }

        private void OnNeedFulfilled(NeedType needType)
        {
            if (!_waitingForNeedsFulfill)
            {
                return;
            }

            if (needType == targetNeed)
            {
                _needsFulfilledCount++;
            }

            if (_needsFulfilledCount == guestsToWaitFor.Count)
            {
                _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
                _currentDirector = null;

                _waitingForNeedsFulfill = false;
                _needsFulfilledCount = 0;
            }
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