using Guest;
using UnityEngine;
using UnityEngine.Playables;

namespace Timeline
{
    public class TimelineWaitForGuestSitDown : MonoBehaviour
    {
        [SerializeField] private GuestGroup _guestGroup;

        private bool _waitingForSitDown;
        private PlayableDirector _currentDirector;

        private void OnEnable()
        {
            _guestGroup.OnGuestsSitDown += OnGuestsSitDown;
        }

        private void OnDisable()
        {
            _guestGroup.OnGuestsSitDown -= OnGuestsSitDown;
        }

        private void OnGuestsSitDown()
        {
            if (_waitingForSitDown)
            {
                _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
                _currentDirector = null;
                _waitingForSitDown = false;
            }
        }

        public void WaitForSitDown(PlayableDirector director)
        {
            if (!_waitingForSitDown)
            {
                _waitingForSitDown = true;

                _currentDirector = director;
                director.playableGraph.GetRootPlayable(0).SetSpeed(0);
            }
        }
    }
}