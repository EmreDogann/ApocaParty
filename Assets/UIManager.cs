using UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Transform pauseMenu;
    private void OnEnable()
    {
        UIInputManager.OnCancelEvent += SetGamePause;
    }

    private void OnDisable()
    {
        UIInputManager.OnCancelEvent -= SetGamePause;
    }

    public void SetGamePause(bool pauseState)
    {
        pauseMenu.gameObject.SetActive(pauseState);
            
        if (pauseState)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
}
