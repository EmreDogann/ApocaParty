using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TextSwitcherBehaviour))]
public class TextSwitcherDrawer : PropertyDrawer
{
    private float _textAreaHeight;
    private const float PADDING = 3.0f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 4;
        return fieldCount * EditorGUIUtility.singleLineHeight + _textAreaHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty keepTextProp = property.FindPropertyRelative("keepTextOnFinish");
        SerializedProperty colorProp = property.FindPropertyRelative("color");
        SerializedProperty fontSizeProp = property.FindPropertyRelative("fontSize");
        SerializedProperty textProp = property.FindPropertyRelative("text");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, keepTextProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight + PADDING;
        EditorGUI.PropertyField(singleFieldRect, colorProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight + PADDING;
        EditorGUI.PropertyField(singleFieldRect, fontSizeProp);

        position.y = singleFieldRect.y + EditorGUIUtility.singleLineHeight + PADDING;
        position.height = _textAreaHeight + EditorGUIUtility.singleLineHeight;
        GUIStyle myStyle = new GUIStyle(EditorStyles.textArea);
        textProp.stringValue = EditorGUI.TextArea(position, textProp.stringValue, myStyle);

        // Set new Text height
        _textAreaHeight = myStyle.CalcHeight(new GUIContent(textProp.stringValue), EditorGUIUtility.currentViewWidth);
    }
}