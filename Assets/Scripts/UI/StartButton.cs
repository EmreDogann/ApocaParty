using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class StartButton : MenuButton
    {
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}