using GuestRequests;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineWaitForRequest : MonoBehaviour
{
    [SerializeField] private Request request;
    private bool _waitingForRequest;
    private PlayableDirector _currentDirector;

    private void OnEnable()
    {
        request.OnRequestCompleted += OnRequestFinished;
    }

    private void OnDisable()
    {
        request.OnRequestCompleted -= OnRequestFinished;
    }

    private void OnRequestFinished()
    {
        if (_waitingForRequest)
        {
            _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
            _currentDirector = null;
            _waitingForRequest = false;
        }
    }

    public void WaitForRequest(PlayableDirector director)
    {
        if (!_waitingForRequest)
        {
            _waitingForRequest = true;

            _currentDirector = director;
            director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        }
    }
}