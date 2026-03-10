
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;

        if (ShouldShow(property, showIf))
            EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        return ShouldShow(property, showIf)
            ? EditorGUI.GetPropertyHeight(property, label)
            : 0;
    }

    private bool ShouldShow(SerializedProperty property, ShowIfAttribute showIf)
    {
        foreach (string boolName in showIf.conditionBools)
        {
            string propertyPath = property.propertyPath;

            // Quitar el nombre del campo actual
            int lastDot = propertyPath.LastIndexOf('.');
            string parentPath = lastDot >= 0
                ? propertyPath.Substring(0, lastDot)
                : "";

            string conditionPath = string.IsNullOrEmpty(parentPath)
                ? boolName
                : parentPath + "." + boolName;

            SerializedProperty conditionProperty =
                property.serializedObject.FindProperty(conditionPath);

            if (conditionProperty == null ||
                conditionProperty.propertyType != SerializedPropertyType.Boolean ||
                !conditionProperty.boolValue)
            {
                return false;
            }
        }

        return true;
    }
}
#endif