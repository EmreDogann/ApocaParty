using Interactions;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineWaitForInteract : MonoBehaviour
{
    [SerializeField] private InteractableBase interactable;
    private bool _waitingForInteract;
    private PlayableDirector _currentDirector;

    private void OnEnable()
    {
        interactable.OnInteracted += OnInteracted;
    }

    private void OnDisable()
    {
        interactable.OnInteracted -= OnInteracted;
    }

    private void OnInteracted()
    {
        if (_waitingForInteract)
        {
            _currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1);
            _currentDirector = null;
            _waitingForInteract = false;
        }
    }

    public void WaitForInteract(PlayableDirector director)
    {
        if (!_waitingForInteract)
        {
            _waitingForInteract = true;

            _currentDirector = director;
            director.playableGraph.GetRootPlayable(0).SetSpeed(0);
        }
    }
}