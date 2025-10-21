using System;
using System.Collections;
using System.Collections.Generic;
using Character.Settings;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
[CreateAssetMenu(menuName = "Settings/GenericSettings")]
[MovedFrom(true, null, null, "GroupSettings")]
public class GroupValues : ScriptableObject
{
    public List<SettingField> fields = new();
    public T GetValue<T>(string field, string name)
    {
        var f = fields.Find(f => f.fieldName == field);
        var entry = f?.entries.Find(e => e.name == name);
        return entry != null ? (T)entry.value.GetValue() : default;
    }
    public T GetValue<T>(string name)
    {
        foreach (var field in fields)
        {
            var entry = field.entries.Find(e => e.name == name);
            if (entry != null)
            {
                return (T)entry.value.GetValue();
            }
        }
        throw new KeyNotFoundException($"No se encontr� ning�n valor con el nombre '{name}' en los campos.");
    }
    public object GetValue(string name)
    {
        foreach (var field in fields)
        {
            var entry = field.entries.Find(e => e.name == name);
            if (entry != null)
                return entry.value.GetValue();
        }
        throw new KeyNotFoundException($"No se encontr� ning�n valor con el nombre '{name}' en los campos.");
    }
    public void SetValue<T>(string field, string name, T newValue)
    {
        var f = fields.Find(f => f.fieldName == field);
        var entry = f?.entries.Find(e => e.name == name);
        entry?.value.SetValue(newValue);
    }
    public void SetValue<T>(string name, T newValue)
    {
        foreach (var field in fields)
        {
            var entry = field.entries.Find(e => e.name == name);
            if (entry != null)
            {
                entry.value.SetValue(newValue);
                return; // Salimos cuando encontramos y actualizamos el valor
            }
        }
        // Si quieres, aqu� podr�as lanzar excepci�n o log si no se encontr� el nombre
        throw new KeyNotFoundException($"No se encontr� ning�n valor con el nombre '{name}' en los campos.");
    }
    public void SetEntryValue(SettingEntry newEntry)
    {
        foreach (var field in fields)
        {
            var entry = field.entries.Find(e => e.name == newEntry.name);
            if (entry != null)
            {
                entry.value = newEntry.value;
                return;
            }
        }

        Debug.LogWarning($"[GroupSettings] No se encontr� la entrada '{newEntry.name}' para actualizar.");
    }
    public void SetEntryValue(int fieldIndex, int entryIndex, object newValue)
    {
        if (fieldIndex >= 0 && fieldIndex < fields.Count &&
            entryIndex >= 0 && entryIndex < fields[fieldIndex].entries.Count)
        {
            fields[fieldIndex].entries[entryIndex].value = (SettingValue)newValue;
        }
        else
        {
            Debug.LogWarning("[GroupSettings] �ndices fuera de rango al hacer SetEntryValue.");
        }
    }

    public GroupValues Clone()
    {
        var clone = ScriptableObject.CreateInstance<GroupValues>();
        clone.fields = new List<SettingField>();
        foreach (var field in fields)
        {
            clone.fields.Add(field.Clone());
        }
        return clone;
    }

    public void CopyFrom(GroupValues other)
    {
        fields.Clear();
        foreach (var field in other.fields)
        {
            fields.Add(field.Clone());
        }
    }
}
#region INDIVIDUALELEMENT
[Serializable]
public abstract class SettingValue
{
    public abstract object GetValue();
    public abstract void SetValue(object value);
    public abstract VALUE_TYPE GetValueType();
    public abstract SettingValue Clone();

}
[Serializable]
public class BoolSettingValue : SettingValue
{
    public bool value;

    public override object GetValue() => value;
    public override void SetValue(object val) => value = Convert.ToBoolean(val);
    public override VALUE_TYPE GetValueType() => VALUE_TYPE.BOOL;
    public override SettingValue Clone()
    {
        return new BoolSettingValue { value = this.value };
    }
    public override bool Equals(object obj)
    {
        if (obj is BoolSettingValue other) return value == other.value;
        return false;
    }
}

[Serializable]
public class FloatSettingValue : SettingValue
{
    public float value;

    public override object GetValue() => value;
    public override void SetValue(object val) => value = Convert.ToSingle(val);
    public override VALUE_TYPE GetValueType() => VALUE_TYPE.FLOAT;
    public override SettingValue Clone()
    {
        return new FloatSettingValue { value = this.value };
    }
    public override bool Equals(object obj)
    {
       if(obj is FloatSettingValue other) return value == other.value;
       return false;
    }
}

[Serializable]
public class StringSettingValue : SettingValue
{
    public string value;

    public override object GetValue() => value;
    public override void SetValue(object val) => value = val?.ToString();
    public override VALUE_TYPE GetValueType() => VALUE_TYPE.STRING;
    public override SettingValue Clone()
    {
        return new StringSettingValue { value = this.value };
    }
    public override bool Equals(object obj)
    {
        if (obj is BoolSettingValue other) return value.Equals(other.value);
        return false;
    }
}
[Serializable]
public class SettingEntry
{
    public string name;
    public VALUE_TYPE type;
    [SerializeReference] public SettingValue value;
    public SettingEntry Clone()
    {
        var newEntry = new SettingEntry();
        newEntry.name = name;
        newEntry.type = type;
        newEntry.value = value.Clone();
        return newEntry;
    }
    public override bool Equals(object obj)
    {
        if (obj is SettingEntry other)
        {
            return name==other.name && type==other.type&&value.Equals(other.value);

        }
        return false;
    }

}
#endregion


#region FIELD
[Serializable]
public class SettingField
{
    public string fieldName;
    public List<SettingEntry> entries = new();
    public SettingField Clone()
    {
        var newField = new SettingField();
        newField.fieldName = fieldName;
        newField.entries = new List<SettingEntry>();
        foreach (var entry in entries)
        {
            newField.entries.Add(entry.Clone());
        }
        return newField;
    }

}
#endregion