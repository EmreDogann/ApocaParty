using System;
using System.Reflection;
using GuestRequests;
using GuestRequests.Jobs;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Job))]
public class JobsGenericDrawer : PropertyDrawer
{
    private const float SINGLE_LINE_PADDING = 2.0f;
    // For a simple example, I'm hard-coding the field types as enum values.
    // A more robust approach would use reflection to find all matching types.
    public enum FieldType
    {
        None,
        MoveToTarget,
        PlaceAtTarget,
        Cook,
        ChangeMusic,
        FixBunting,
        ChangeSprite,
        FixPowerOutage
    }

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
                    height += EditorGUI.GetPropertyHeight(nested, nested.isExpanded) + SINGLE_LINE_PADDING;
                } while (nested.NextVisible(false) && !SerializedProperty.EqualContents(nested, endMarker));
            }
        }

        return height + 12.0f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw a drop-down to select what type of field we want.
        FieldType currentType = GetTypeID(property);

        Rect enumRect = position;
        enumRect.height = EditorGUIUtility.singleLineHeight;
        FieldType newType = (FieldType)EditorGUI.EnumPopup(enumRect, label, currentType);
        if (newType != currentType)
        {
            // If the user changes the field type, assign a new instance of that type.
            Job job = MakeDefault(newType);
            job.GetType().GetProperty(nameof(Job.JobName))
                ?.SetValue(job, ObjectNames.NicifyVariableName(job.GetType().Name));

            property.isExpanded = true;

            property.managedReferenceValue = job;
            property.serializedObject.ApplyModifiedProperties();
        }

        // Draw a fold-out if this type has child properties.
        if (property.hasVisibleChildren)
        {
            Rect foldZone = position;
            foldZone.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(foldZone, property.isExpanded, " ", true);

            // Draw those properties, if folded-out.
            if (property.isExpanded)
            {
                DrawChildren(position, property);
            }
        }

        EditorGUI.EndProperty();
    }

    private void DrawChildren(Rect position, SerializedProperty property)
    {
        // Inset our drawing inside the folded-out region.
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel++;

        position.y += EditorGUIUtility.singleLineHeight + SINGLE_LINE_PADDING + 3.0f;
        position.height = EditorGUIUtility.singleLineHeight;

        // Note where this property ends and the next one begins.
        SerializedProperty endMarker = property.Copy();
        endMarker.NextVisible(false);

        // Descend into the first child property.
        SerializedProperty nested = property.Copy();
        nested.NextVisible(true);
        do
        {
            float height = 0.0f;
            // if (!IsThisPropertyMustBeDrawnWithYourPropertyDrawer(nested))
            // {
            //     typeof(EditorGUI)
            //         .GetMethod("DefaultPropertyField", BindingFlags.NonPublic | BindingFlags.Static)
            //         ?.Invoke(null, new object[] { position, nested, new GUIContent(nested.displayName) });
            //
            //     height = (float)typeof(EditorGUI)
            //         .GetMethod("GetPropertyHeightInternal", BindingFlags.NonPublic | BindingFlags.Static)
            //         ?.Invoke(null,
            //             new object[] { nested, new GUIContent(nested.displayName), nested.isExpanded })!;
            // }
            // else
            // {
            // For each child property, draw its inspector widgets in a stack.
            EditorGUI.PropertyField(position, nested);
            height = EditorGUI.GetPropertyHeight(nested, nested.isExpanded);
            // }

            // Advance the positioning of the next property by how much space this one took up.
            position.y += height + SINGLE_LINE_PADDING;
            position.height = height;
        } while (nested.NextVisible(false) && !SerializedProperty.EqualContents(nested, endMarker));

        // Return the indent to where we found it.
        EditorGUI.indentLevel = indent;
    }

    // From: https://github.com/slavniyteo/one-line/issues/15
    private static bool IsThisPropertyMustBeDrawnWithYourPropertyDrawer(SerializedProperty property)
    {
        Type fieldType = GetPropertyType(property);
        return fieldType == typeof(Job);
    }

    private static Type GetPropertyType(SerializedProperty property)
    {
        string[] path = property.propertyPath.Split('.');

        bool isFailed = false;
        Type type = property.serializedObject.targetObject.GetType();
        for (int i = 0; i < path.Length; i++)
        {
            FieldInfo field = type?.GetField(path[i], BindingFlags.Public
                                                      | BindingFlags.NonPublic
                                                      | BindingFlags.Instance);

            if (field != null)
            {
                type = field.FieldType;
            }
            else
            {
                isFailed = true;
            }

            int next = i + 1;
            if (next < path.Length && path[next] == "Array")
            {
                i += 2;
                if (type != null && type.IsArray)
                {
                    type = type.GetElementType();
                }
                else
                {
                    type = type?.GetGenericArguments()[0];
                }
            }
        }

        return isFailed ? type : null;
    }

    // Helper method to get the type of field as an enum value.
    private static FieldType GetTypeID(SerializedProperty property)
    {
        string typeName = property.managedReferenceFullTypename;
        if (string.IsNullOrEmpty(typeName))
        {
            return FieldType.None;
        }

        typeName = typeName.Substring(typeName.LastIndexOf('.') + 1);

        return (FieldType)Enum.Parse(typeof(FieldType), typeName);
    }

    // Helper method to make a new instance of a field class given an enum type ID.
    private static Job MakeDefault(FieldType fieldType)
    {
        switch (fieldType)
        {
            case FieldType.MoveToTarget: return new MoveToTarget();
            case FieldType.PlaceAtTarget: return new PlaceAtTarget();
            case FieldType.Cook: return new Cook();
            case FieldType.ChangeMusic: return new ChangeMusic();
            case FieldType.FixBunting: return new FixBunting();
            case FieldType.ChangeSprite: return new ChangeSprite();
            case FieldType.FixPowerOutage: return new FixPowerOutage();
            case FieldType.None: return null;
            default:
                Debug.Log($"Unknown field type {fieldType}.");
                return null;
        }
    }
}