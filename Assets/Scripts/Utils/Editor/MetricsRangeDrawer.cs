using Needs;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    [CustomPropertyDrawer(typeof(MetricsRangeAttribute))]
    public class MetricsRangeDrawer : PropertyDrawer
    {
        private bool _isFirstRun = true;
        private SerializedProperty _hungerProperty;
        private SerializedProperty _thirstProperty;
        private SerializedProperty _enjoymentProperty;
        private SerializedProperty _movementProperty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;

            if (property.isExpanded)
            {
                // Note where this property ends and the next one begins.
                SerializedProperty endMarker = property.Copy();
                endMarker.NextVisible(false);

                // Descend into the first nested child property.
                SerializedProperty nested = property.Copy();
                if (nested.NextVisible(true) && !SerializedProperty.EqualContents(nested, endMarker))
                {
                    do
                    {
                        // For each child property, accumulate its height.
                        height += EditorGUI.GetPropertyHeight(nested, nested.isExpanded) + 2.0f;
                    } while (nested.NextVisible(false) && !SerializedProperty.EqualContents(nested, endMarker));
                }
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            if (fieldInfo.FieldType != typeof(NeedMetrics))
            {
                EditorGUI.LabelField(position, label.text, "Can only use on class NeedMetrics.");
                return;
            }

            if (attribute is MetricsRangeAttribute range)
            {
                if (_isFirstRun)
                {
                    _hungerProperty = property.FindPropertyRelative(nameof(NeedMetrics.hunger));
                    _thirstProperty = property.FindPropertyRelative(nameof(NeedMetrics.thirst));
                    _enjoymentProperty = property.FindPropertyRelative(nameof(NeedMetrics.enjoyment));
                    _movementProperty = property.FindPropertyRelative(nameof(NeedMetrics.movement));
                    _isFirstRun = false;
                }

                float yIncrement = EditorGUIUtility.singleLineHeight + 2.0f;
                position.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(position, label);

                if (!property.hasVisibleChildren)
                {
                    return;
                }

                Rect foldZone = position;
                foldZone.height = EditorGUIUtility.singleLineHeight;
                property.isExpanded = EditorGUI.Foldout(foldZone, property.isExpanded, " ", true);

                // Don't draw those properties, if folded-in.
                if (!property.isExpanded)
                {
                    return;
                }

                EditorGUI.indentLevel++;

                position.y += yIncrement;
                EditorGUI.Slider(position, _hungerProperty, range.Min, range.Max, _hungerProperty.displayName);

                position.y += yIncrement;
                EditorGUI.Slider(position, _thirstProperty, range.Min, range.Max, _thirstProperty.displayName);

                position.y += yIncrement;
                EditorGUI.Slider(position, _enjoymentProperty, range.Min, range.Max, _enjoymentProperty.displayName);

                position.y += yIncrement;
                EditorGUI.Slider(position, _movementProperty, range.Min, range.Max, _movementProperty.displayName);

                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Error retrieving range attribute params.");
            }

            EditorGUI.EndProperty();
        }
    }
}