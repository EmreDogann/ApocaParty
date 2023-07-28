using UnityEngine;
using Utils;

public class SceneLoader : MonoBehaviour
{
    public SceneReference sceneToLoad;
    public bool loadOnAwake;

    private void Awake()
    {
        if (loadOnAwake)
        {
            SceneLoaderManager.Instance.LoadScene(sceneToLoad.ScenePath, true);
        }
    }

    public void LoadScene()
    {
        // Maybe can trigger transition here, and then load scene? (e.g. looney toons circle)
        SceneLoaderManager.Instance.SwapActiveScene(sceneToLoad.ScenePath);
    }
}