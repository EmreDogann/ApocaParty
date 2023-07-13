using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class SceneLoader : MonoBehaviour
{
    public SceneReference sceneToLoad;

    public void LoadScene()
    {
        // Maybe can trigger transition here, and then load scene? (e.g. looney toons circle)
        SceneManager.LoadScene(sceneToLoad.ScenePath, LoadSceneMode.Single);
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