using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    public static SceneLoaderManager Instance { get; private set; }
    public AudioListener loadingAudioListener;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnload;

        loadingAudioListener.enabled = false;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnload;
    }


#if UNITY_STANDALONE && !UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void LoadStartingSceneAtStartup()
    {
        if (SceneManager.sceneCount == 1 && SceneManager.GetActiveScene().buildIndex == 0)
        {
            Instance.LoadScene(SceneUtility.GetScenePathByBuildIndex(1), true);
        }
    }
#endif

    public void LoadScene(string scenePath, bool setActive)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);

        void AsyncDelegate(AsyncOperation obj)
        {
            if (obj.isDone)
            {
                if (setActive)
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scenePath));
                }
            }
            else
            {
                Debug.LogError($"Error (SceneLoaderManager): Scene {scenePath} failed loading");
            }

            asyncOperation.completed -= AsyncDelegate;
        }

        asyncOperation.completed += AsyncDelegate;
    }

    public void UnloadScene(Scene scene)
    {
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(scene.path, UnloadSceneOptions.None);

        void AsyncDelegate(AsyncOperation obj)
        {
            if (obj.isDone)
            {
                Resources.UnloadUnusedAssets();
            }
            else
            {
                Debug.LogError($"Error (SceneLoaderManager): Scene {scene.path} failed unloading");
            }

            asyncOperation.completed -= AsyncDelegate;
        }

        asyncOperation.completed += AsyncDelegate;
    }

    public void SwapActiveScene(string scenePath)
    {
        StartCoroutine(SwapScenes(scenePath));
    }

    private IEnumerator SwapScenes(string scenePath)
    {
        MouseCursor.CursorActive(false);
        // This is required to fix some weird issue. https://issuetracker.unity3d.com/issues/loadsceneasync-allowsceneactivation-flag-is-ignored-in-awake?page=1#comments
        yield return null;

        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene(), UnloadSceneOptions.None);
        loadingAudioListener.enabled = true;
        yield return SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);
        loadingAudioListener.enabled = false;

        yield return Resources.UnloadUnusedAssets();
        MouseCursor.CursorActive(true);

        SceneManager.SetActiveScene(SceneManager.GetSceneByPath(scenePath));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {}

    private void OnSceneUnload(Scene scene) {}
}