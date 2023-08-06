using Arrivals;
using UnityEngine;
using UnityEngine.Playables;

namespace Timeline
{
    public class TimelineWaitForGuestArrival : MonoBehaviour
    {
        private bool _waitingForArrival;
        private PlayableDirector _currentDirector;

        private void OnEnable()
        {
            ArrivalSystem.OnGuestsArrived += OnGuestsArrived;
        }

        private void OnDisable()
        {
            ArrivalSystem.OnGuestsArrived -= OnGuestsArrived;
        }

        private void OnGuestsArrived()
        {
            if (_waitingForArrival)
            {
                _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
                _currentDirector = null;
                _waitingForArrival = false;
            }
        }

        public void WaitForArrival(PlayableDirector director)
        {
            if (!_waitingForArrival)
            {
                _waitingForArrival = true;

                _currentDirector = director;
                director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            }
        }
    }
}