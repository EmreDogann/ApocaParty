using Needs;
using UnityEngine;
using UnityEngine.Playables;

namespace Timeline
{
    public class TimelineWaitForNeedsReveal : MonoBehaviour
    {
        [SerializeField] private NeedSystem needSystem;

        private bool _waitingForNeedReveal;
        private PlayableDirector _currentDirector;

        private void OnEnable()
        {
            needSystem.OnNeedsResolved += OnNeedsResolved;
        }

        private void OnDisable()
        {
            needSystem.OnNeedsResolved -= OnNeedsResolved;
        }

        private void OnNeedsResolved()
        {
            if (!_waitingForNeedReveal)
            {
                return;
            }

            _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
            _currentDirector = null;

            _waitingForNeedReveal = false;
        }

        public void WaitForNeedReveal(PlayableDirector director)
        {
            if (!_waitingForNeedReveal)
            {
                _waitingForNeedReveal = true;

                _currentDirector = director;
                director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            }
        }
    }
}