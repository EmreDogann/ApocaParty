using UnityEngine;

public class UnpauseSceneOnAwake : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 1.0f;
    }
}