#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    [CustomPropertyDrawer(typeof(NavMeshSelectorAttribute))]
    public class NavMeshMaskSelector : PropertyDrawer
    {
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty serializedProperty, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            string[] navMeshAreaNames = GameObjectUtility.GetNavMeshAreaNames();
            int navMeshArea = serializedProperty.intValue;
            int selectedIndex = -1;
            for (int i = 0; i < navMeshAreaNames.Length; i++)
            {
                if (GameObjectUtility.GetNavMeshAreaFromName(navMeshAreaNames[i]) == navMeshArea)
                {
                    selectedIndex = i;
                    break;
                }
            }

            int num = EditorGUI.Popup(position, label.text, selectedIndex, navMeshAreaNames);
            if (EditorGUI.EndChangeCheck())
            {
                int navMeshAreaFromName = GameObjectUtility.GetNavMeshAreaFromName(navMeshAreaNames[num]);
                serializedProperty.intValue = navMeshAreaFromName;
            }
        }
    }
}
#endif