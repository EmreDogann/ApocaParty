using UnityEngine;

public class DisabledOnAwake : MonoBehaviour
{
    private void Awake()
    {
        transform.gameObject.SetActive(false);
    }
}