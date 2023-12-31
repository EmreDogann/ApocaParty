using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasHiddenOnAwake : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<CanvasGroup>().alpha = 0.0f;
        }
    }
}