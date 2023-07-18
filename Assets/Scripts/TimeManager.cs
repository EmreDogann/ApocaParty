using Events;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private BoolEventListener _onGamePauseListener;

    private void OnEnable()
    {
        _onGamePauseListener.Response.AddListener(OnPauseToggle);
    }

    private void OnDisable()
    {
        _onGamePauseListener.Response.RemoveListener(OnPauseToggle);
    }

    private void OnPauseToggle(bool isPaused)
    {
        if (isPaused)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }

    public void Unpause(float timeScale = 1.0f)
    {
        Time.timeScale = timeScale;
    }

    public void Pause(float timeScale = 0.0f)
    {
        Time.timeScale = timeScale;
    }
}