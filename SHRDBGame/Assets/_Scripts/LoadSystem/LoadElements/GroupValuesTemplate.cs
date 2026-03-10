using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "LoadSystem/GroupValuesTemplate")]
public class GroupValuesTemplate : ScriptableObject
{
    [UniqueReference]
    public GroupValues groupValuesReference;
    [SerializeField]
    public List<SettingField> fields { get => defaultFields; }

    [SerializeField] private List<SettingField> defaultFields;

    [Button("Set default values from reference")]
    [ContextMenu("Set default values from reference")]
    public void SetDefaultValuesInTemplate()
    {

        defaultFields = new List<SettingField>();
        if (groupValuesReference == null)
        {
            Debug.LogWarning("Group values to reference is null");
            return;
        }

        foreach (var field in groupValuesReference.fields)
        {
            defaultFields.Add(field.Clone());
        }

    }
    [ContextMenu("Log first field")]
    public void logfirstfield()
    {
        Debug.Log(defaultFields[0].entries[0].value);
    }
    public void SetDefaultValuesInSO()
    {

        if (groupValuesReference == null)
        {
            Debug.LogError("Target GroupValues is null");
            return;
        }

        if (defaultFields == null || defaultFields.Count == 0)
        {
            Debug.LogWarning("No default fields set in template. Run SetDefaultValuesInTemplate first.");
            return;
        }

        groupValuesReference.fields = new List<SettingField>();
        foreach (var field in defaultFields)
        {
            groupValuesReference.fields.Add(field.Clone());
        }

#if UNITY_EDITOR
        EditorUtility.SetDirty(groupValuesReference);
        AssetDatabase.SaveAssets();
#endif
        Debug.Log($"Applied template defaults to {groupValuesReference.name}");
    }
    //The template only gives values, it is read only
    public T GetValue<T>(string field, string name)
    {
        var f = defaultFields.Find(f => f.fieldName == field);
        var entry = f?.entries.Find(e => e.name == name);
        return entry != null ? (T)entry.value.GetValue() : default;
    }
    public GroupValuesTemplate Clone()
    {
        var clone = ScriptableObject.CreateInstance<GroupValuesTemplate>();

        clone.CreateFields();
        clone.groupValuesReference = groupValuesReference;
        foreach (var field in fields)
        {
            clone.fields.Add(field.Clone());
        }

        return clone;
    }
    public void CopyFrom(GroupValuesTemplate other)
    {

        defaultFields.Clear();
        groupValuesReference = other.groupValuesReference;
        foreach (var field in other.fields)
        {
            fields.Add(field.Clone());
        }


    }
    public void CreateFields()
    {
        defaultFields = new List<SettingField>();
    }
    public void ResetFields()
    {

        foreach (var field in fields)
            foreach (var entry in field.entries)
                entry.value = SettingValueFactory.Create(entry.type);

    }
}