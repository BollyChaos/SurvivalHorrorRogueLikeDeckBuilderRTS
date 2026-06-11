#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(SettingEntry))]
public class SettingEntryDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var typeProp = property.FindPropertyRelative("type");
        var valueProp = property.FindPropertyRelative("value");

        float line = EditorGUIUtility.singleLineHeight;
        float height = line * 2 + 4; // name + type

        VALUE_TYPE selected = (VALUE_TYPE)typeProp.enumValueIndex;

        if (selected == VALUE_TYPE.STRING)
        {
            height += line * 3 + 6;
        }
        else if (valueProp != null && valueProp.managedReferenceValue != null)
        {
            height += EditorGUI.GetPropertyHeight(valueProp, true) + 4;
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        SerializedProperty nameProp = property.FindPropertyRelative("name");
        SerializedProperty typeProp = property.FindPropertyRelative("type");
        SerializedProperty valueProp = property.FindPropertyRelative("value");
        
        float lineHeight = EditorGUIUtility.singleLineHeight;

        float y = position.y;

        // Campo "name"
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), nameProp);
        y += lineHeight + 2;

        // Campo "type" con detección de cambio
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(new Rect(position.x, y, position.width, lineHeight), typeProp);
        VALUE_TYPE selected = (VALUE_TYPE)typeProp.enumValueIndex;
        if (EditorGUI.EndChangeCheck())
        {

            if (valueProp != null && valueProp.managedReferenceValue != null)
            {
                lineHeight += EditorGUI.GetPropertyHeight(valueProp, true) + 4f;
            }
            SettingValue newInstance = selected switch
            {
                VALUE_TYPE.BOOL => new BoolSettingValue(),
                VALUE_TYPE.FLOAT => new FloatSettingValue(),
                VALUE_TYPE.STRING => new StringSettingValue(),
                VALUE_TYPE.INT => new IntSettingValue(),
                VALUE_TYPE.DOUBLE => new DoubleSettingValue(),
                VALUE_TYPE.LONG => new LongSettingValue(),
                VALUE_TYPE.SHORT => new ShortSettingValue(),
                VALUE_TYPE.BYTE => new ByteSettingValue(),
                VALUE_TYPE.VECTOR2 => new Vector2SettingValue(),

                _ => null
            };

            if (newInstance != null)
            {
                valueProp.managedReferenceValue = newInstance;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
        y += lineHeight + 4;

        // Campo "value" (solo si hay instancia)
        // if (valueProp != null && valueProp.managedReferenceValue != null)
        // {
        //     float valueHeight = EditorGUI.GetPropertyHeight(valueProp, true);
        //     EditorGUI.PropertyField(new Rect(position.x, y, position.width, valueHeight), valueProp, true);
        // }

        if (valueProp != null && valueProp.managedReferenceValue != null)
        {
            if (selected == VALUE_TYPE.STRING)
            {
                var valueField = valueProp.FindPropertyRelative("value");
                GUIStyle wrapStyle = new GUIStyle(EditorStyles.textArea)
                {
                    wordWrap = true
                };

                Rect area = new Rect(position.x, y, position.width, lineHeight * 3);
                valueField.stringValue = EditorGUI.TextArea(area, valueField.stringValue, wrapStyle);
            }
            else
            {
                float valueHeight = EditorGUI.GetPropertyHeight(valueProp, true);
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, valueHeight), valueProp, true);
            }
        }
    }
    static int GetArrayIndex(SerializedProperty property)
    {
        string path = property.propertyPath;

        int start = path.LastIndexOf("[") + 1;
        int end = path.LastIndexOf("]");

        string indexStr = path.Substring(start, end - start);
        return int.Parse(indexStr);
    }
    #region GROUPVALUESLIST



ReorderableList list;

void SetupList(SerializedProperty entriesProp)
{
    list = new ReorderableList(
        entriesProp.serializedObject,
        entriesProp,
        draggable: true,
        displayHeader: true,
        displayAddButton: true,
        displayRemoveButton: true
    );

    // Header
    list.drawHeaderCallback = (Rect rect) =>
    {
        EditorGUI.LabelField(rect, "Entries");
    };

    // Element drawer
    list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
    {
        SerializedProperty element = entriesProp.GetArrayElementAtIndex(index);
        EditorGUI.PropertyField(rect, element, true);
    };

    // Altura por elemento
    list.elementHeightCallback = (int index) =>
    {
        SerializedProperty element = entriesProp.GetArrayElementAtIndex(index);
        return EditorGUI.GetPropertyHeight(element, true) + 4f;
    };
    list.onAddCallback = (ReorderableList l) =>
   {
       // Aumentar tamaño del array
       entriesProp.arraySize++;
       entriesProp.serializedObject.ApplyModifiedProperties();

       int newIndex = entriesProp.arraySize - 1;
       SerializedProperty newElement = entriesProp.GetArrayElementAtIndex(newIndex);

       // Crear un SettingEntry completamente nuevo
       var newEntry = new SettingEntry
       {
           name = "New Entry",
           type = VALUE_TYPE.INT, // valor por defecto
           value = SettingValueFactory.Create(VALUE_TYPE.INT)
       };

       newElement.managedReferenceValue = newEntry;
       entriesProp.serializedObject.ApplyModifiedProperties();
   };
}
#endregion

}


#endif