using UnityEditor;
using UnityEngine;
using System.Reflection;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.conditionBool);

        if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean && conditionProperty.boolValue)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        SerializedProperty conditionProperty = property.serializedObject.FindProperty(showIf.conditionBool);

        if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Boolean && conditionProperty.boolValue)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        else
        {
            return 0;
        }
    }
}
