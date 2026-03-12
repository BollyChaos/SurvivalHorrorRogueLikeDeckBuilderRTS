using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Threading.Tasks;
using UnityEditor.Playables;





#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
#endif

[Serializable]
public class ALoader
{


#if UNITY_EDITOR


    protected string resourcePath;

    // [Header("Debug")]
    // [SerializeField] protected bool debug = false;
    // [ShowIf("debug")]
    // [SerializeField] protected bool CreateUnEncryptedJsonCopy = true;


#endif

    [Header("SO Path")]
    [SerializeField] protected string soPath = "Assets/Resources/LoadSystem/SavedFiles/";
    [SerializeField] protected string baseName = "Game";

    protected string soName => baseName + ".asset";
    protected string jsonFileName => baseName + ".json";
    [ReadOnly]
    [SerializeField] EncryptionMethod encryptionMethod = EncryptionMethod.None;
    [ReadOnly][SerializeField] string password = "";


    [SerializeField]
    [ExposedScriptableObject]
    protected GroupValues values;
    public void SetEncrytionSettings(EncryptionMethod eM, string passw)
    {
        encryptionMethod = eM;
        password = passw;
    }
    //summary>
    //Change the asset name for both SO and JSON, no extension needed
    ///</summary>

    public void ChangeAssetName(string newName)
    {
        baseName = newName;
    }
    public string GetCurrentName()
    {
        return baseName;
    }
    #region MAINTHREAD
    // ---------------------------------------------------------------------------------------
    // LOAD
    // ---------------------------------------------------------------------------------------
    [ContextMenu("Load Data")]
    public GroupValues LoadValues()
    {
        // Solo cargamos el SO una vez
        if (values == null)
        {
#if UNITY_EDITOR
            string soFullPath = Path.Combine(soPath, soName);

            if (!File.Exists(soFullPath))
            {
                Debug.LogWarning("No existe el archivo SO en: " + soFullPath);
            }
            values = AssetDatabase.LoadAssetAtPath<GroupValues>(soFullPath);
            if (values == null)
            {
                Debug.LogError("No se han encontrado los valores en: " + soFullPath);
            }
#else
            // En build los ScriptableObjects NO se pueden cargar desde Assets
            // Si lo quieres cargar, debe estar en Resources
            string resourceName = Path.GetFileNameWithoutExtension(soName);
            values = Resources.Load<GroupValues>(Path.Combine(GetPathFromResources(soPath),resourceName));
             if (values == null)
        {
            Debug.LogError("[Loader] No se pudo cargar el SO base en :"+resourceName);
            return null;
        }
#endif
        }

        if (encryptionMethod != EncryptionMethod.None)
        {
            var jsonText = JsonEncrypter.DecryptFromFile(GetJsonPath(), password, encryptionMethod);
            //            Debug.Log(jsonText);

            LoadFromJsonString(jsonText);
        }
        else
            // Cargar valores desde JSON
            LoadFromJsonFile();
#if UNITY_EDITOR
        EditorUtility.SetDirty(values);
#endif
        return values.Clone();
    }
    public void SetGroupValues(GroupValues gv)
    {
        values = gv.Clone();
    }
    public void RemoveLoadedValues()
    {
        values = null;
    }
    public static string GetPathFromResources(string fullPath)
    {
        string keyword = "Resources/";
        int index = fullPath.IndexOf(keyword);
        if (index >= 0)
        {
            // Tomamos todo a partir del final de "Resources/"
            return fullPath.Substring(index + keyword.Length);
        }
        else
        {
            Debug.LogWarning("Path doesn't contain 'Resources/'");
            return fullPath;
        }
    }

    // ---------------------------------------------------------------------------------------
    // GUARDAR
    // ---------------------------------------------------------------------------------------
    [ContextMenu("Save Data")]
    public void SaveValues()
    {
        if (values == null) { Debug.LogError("[Loader]No values to save"); return; }
        // if (debug && CreateUnEncryptedJsonCopy)
        //     SaveToJsonFile();
        if (encryptionMethod != EncryptionMethod.None)
            JsonEncrypter.EncryptToFile(GetJsonPath(), GetJsonString(), password, encryptionMethod);
        else
            SaveToJsonFile();

    }
    //shortcut for templates
    public void SaveValues(SerializableGroupSettings sgs)
    {
        if (encryptionMethod != EncryptionMethod.None)
            JsonEncrypter.EncryptToFile(GetJsonPath(), GetJsonString(sgs), password, encryptionMethod);
        else
            SaveToJsonFile();
    }
#if UNITY_EDITOR
    //Used only by buildpreprocesor
    public void SaveValuesInPersistentData()
    {
        if (values == null) { throw new BuildFailedException("[Loader]No values to save"); }

        if (encryptionMethod != EncryptionMethod.None)
            JsonEncrypter.EncryptToFile(Path.Combine(Application.persistentDataPath, jsonFileName), GetJsonString(), password, encryptionMethod);
        else
            SaveToJsonFile(Path.Combine(Application.persistentDataPath, jsonFileName));
    }
    public void SaveValuesInPersistentData(SerializableGroupSettings sgs)
    {
        if (sgs == null) { throw new BuildFailedException("[Loader]No values to save"); }

        if (encryptionMethod != EncryptionMethod.None)
            JsonEncrypter.EncryptToFile(Path.Combine(Application.persistentDataPath, jsonFileName), GetJsonString(sgs), password, encryptionMethod);
        else
            SaveToJsonFile(Path.Combine(Application.persistentDataPath, jsonFileName), sgs);
    }
#endif
    public void SaveValues(GroupValues valuesToSave = null)
    {
        // if(valuesToSave==null) Debug.Log("[Loader] Saving current data");

        if (values == null && valuesToSave == null)
        {
            Debug.LogWarning("[Loader] No values to save.");
            return;
        }
        if (valuesToSave != null)
        {
            if (values != null)//If already had data compare before saving
                if (values.IsTheSame(valuesToSave))
                {
                    Debug.Log("[Loader] The data introduced is the same as the current one. No changes made.");
                    return;
                }
            values = valuesToSave.Clone();
            SaveValues();
        }
        SaveValues();

    }
    public void ResetDefaultValues()
    {
        values.ResetToDefaults();
        SaveToJsonFile();

    }

    // ---------------------------------------------------------------------------------------
    // JSON LOAD
    // ---------------------------------------------------------------------------------------
    protected virtual void LoadFromJsonFile()
    {
        string path = GetJsonPath();

        if (!File.Exists(path))
        {
            Debug.LogWarning("[Loader] JSON doesn't exist in: " + path);
            CreateJsonFile(path);
            return;
        }

        Debug.Log("[Loader] JSON found in: " + path);

        string json = File.ReadAllText(path);
        SerializableGroupSettings sgs = new SerializableGroupSettings();
        JsonUtility.FromJsonOverwrite(json, sgs);

        sgs.ApplyTo(values);
    }
    protected virtual void LoadFromJsonString(string jsonText)
    {

        SerializableGroupSettings sgs = new SerializableGroupSettings();
        JsonUtility.FromJsonOverwrite(jsonText, sgs);

        sgs.ApplyTo(values);
    }
    // ---------------------------------------------------------------------------------------
    // JSON SAVE
    // ---------------------------------------------------------------------------------------
    protected virtual void SaveToJsonFile(string path = null, SerializableGroupSettings sgs = null)
    {
        if (path == null)
            path = GetJsonPath();
        if (sgs == null)
            sgs = new SerializableGroupSettings();
        if (values != null && sgs == null)
            sgs.CopyFrom(values);

        string json = JsonUtility.ToJson(sgs, true);

        File.WriteAllText(path, json);

        Debug.Log("[SettingsSerializer] Saved in " + path);
    }
    protected virtual string GetJsonString()
    {

        SerializableGroupSettings sgs = new SerializableGroupSettings();
        sgs.CopyFrom(values);

        string json = JsonUtility.ToJson(sgs, true);
        return json;
    }
    protected virtual string GetJsonString(SerializableGroupSettings sgs)
    {
        string json = JsonUtility.ToJson(sgs, true);
        return json;
    }

    // ---------------------------------------------------------------------------------------
    // RUTA DEL JSON
    // ---------------------------------------------------------------------------------------
    private string GetJsonPath()
    {
#if UNITY_EDITOR

        return Path.Combine(soPath, jsonFileName);
#else
        // En build: solo persistentDataPath
        return Path.Combine(Application.persistentDataPath, jsonFileName);
#endif
    }

    // ---------------------------------------------------------------------------------------
    // CREAR JSON
    // ---------------------------------------------------------------------------------------
    public void CreateJsonFile(string fullPath)
    {
        string folderPath = Path.GetDirectoryName(fullPath);

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        if (!fullPath.EndsWith(".json"))
            fullPath += ".json";

        if (File.Exists(fullPath))
        {
            Debug.Log($"[Loader] JSON file already exists in: {fullPath}");
            return;
        }

        // Crear JSON por defecto con campos válidos
        SerializableGroupSettings defaults = new SerializableGroupSettings();
        defaults.CopyFrom(values);

        string json = JsonUtility.ToJson(defaults, true);
        File.WriteAllText(fullPath, json);

        Debug.Log($"[Loader] JSON created in: {fullPath}");
    }

    internal void SetValue<T>(string key, T value)
    {
        float start = Time.realtimeSinceStartup;
        if (values == null) return;
        values.SetValue(key, value);
        float end = Time.realtimeSinceStartup;
        Debug.Log("SetValue took: " + ((end - start) * 1000f) + " ms");
#if UNITY_EDITOR

        // Debug.Log("[ALoader] IsDirty: " + EditorUtility.IsDirty(values));
        // UnityEditor.SceneView.RepaintAll();
        // UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

        EditorUtility.SetDirty(values);

#endif
    }

    internal T GetValue<T>(string key)
    {
        if (values == null) return default(T);
        return values.GetValue<T>(key);
    }
    // ---------------------------------------------------------------------------------------
    // RESET TO DEFAULTS
    // ---------------------------------------------------------------------------------------
    [ContextMenu("Reset To Defaults")]
    public void ResetToDefaults()
    {
        if (values == null) return;
        values.ResetToDefaults();
    }
    //////////////////////////////////////////////////////////
    /// 
#if UNITY_EDITOR
    // ruta dentro de Resources
    public bool AutoResolveFromResources()
    {
        string[] guids = AssetDatabase.FindAssets($"t:GroupValues {baseName}");

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // debe estar dentro de Resources
            int resIndex = path.IndexOf("Resources/");
            if (resIndex < 0) continue;

            // cargar asset, not necessary
            // var asset = AssetDatabase.LoadAssetAtPath<GroupValues>(path);
            // if (asset == null) continue;

            // values = asset;

            // carpeta del SO
            soPath = Path.GetDirectoryName(path).Replace("\\", "/") + "/";

            // calcular resourcePath real
            string insideResources = path.Substring(resIndex + "Resources/".Length);
            resourcePath = Path.ChangeExtension(insideResources, null);

            Debug.Log($"[ALoader] Auto-solved:");
            Debug.Log($"SO Path: {soPath}");
            Debug.Log($"Resources Path: {resourcePath}");

            return true;
        }

        Debug.LogError($"[ALoader] No se encontró '{baseName}' dentro de Resources.");
        return false;
    }
#endif
    #endregion
    #region TASKS

    [HideInInspector]
    private bool isSaving;

    public async Task SaveValuesAsync(GroupValues valuesToSave = null)
    {
        if (isSaving) return;
        isSaving = true;

        try
        {
            if (values == null && valuesToSave == null)
                return;

            if (valuesToSave == null)
                valuesToSave = values;

            var clone = valuesToSave.Clone();

            SerializableGroupSettings sgs = new();
            sgs.CopyFrom(clone);

            string json = JsonUtility.ToJson(sgs, true);
            string path = GetJsonPath();

            await Task.Run(() =>
            {
                File.WriteAllText(path, json);
            });
        }
        finally
        {
            isSaving = false;
        }
    }
    public async Task<GroupValues> LoadValuesAsync()
    {
        if (values == null)
        {
            Debug.LogError("[ALoader] No base values assigned.");
            return null;
        }

        string path = GetJsonPath();

        if (!File.Exists(path))
        {
            Debug.LogWarning("[ALoader] JSON not found. Creating default.");
            CreateJsonFile(path);
            return values.Clone();
        }

        // ---- BACKGROUND THREAD (IO only) ----
        string json = await Task.Run(() =>
        {
            return File.ReadAllText(path);
        });

        // ---- MAIN THREAD ----
        SerializableGroupSettings sgs = new();
        JsonUtility.FromJsonOverwrite(json, sgs);

        sgs.ApplyTo(values);

        return values.Clone();
    }
    #endregion
}
#region WRAPPER
// ---------------------------------------------------------------------------------------
// SERIALIZABLE WRAPPER
// ---------------------------------------------------------------------------------------
public class SerializableGroupSettings
{
    public List<SettingField> fields = new();

    public void CopyFrom(GroupValues settings)
    {
        fields.Clear();
        foreach (var f in settings.fields)
        {
            fields.Add(f.Clone());
        }
    }
    public void CopyFrom(List<SettingField> otherFields)
    {
        fields.Clear();
        foreach (var f in otherFields)
        {
            fields.Add(f.Clone());
        }
    }

    public void ApplyTo(GroupValues target)
    {
        target.fields.Clear();
        foreach (var f in fields)
        {
            target.fields.Add(f.Clone());
        }
    }
}
#region XML_SERIALIZATION
public class SerializableGroupValuesXML
{
    public List<SerializableField> fields = new();
    public SerializableGroupValuesXML() { }
    public SerializableGroupValuesXML(List<SerializableField> fs)
    {
        fields = fs;
    }

    public void CopyFrom(GroupValues settings)
    {
        fields.Clear();

        foreach (var f in settings.fields)
        {
            SettingField fieldClone = f.Clone();
            List<SerializableEntry> entriesField = new();

            foreach (var e in fieldClone.entries)
            {
                SettingEntry entryClone = e.Clone();
                SerializableEntry entry = new SerializableEntry(entryClone.name, entryClone.type, entryClone.value.GetValue().ToString());
                entriesField.Add(entry);

            }
            SerializableField fieldToAdd = new SerializableField(fieldClone.fieldName, entriesField);

            fields.Add(fieldToAdd);
        }
    }
    public void CopyFrom(List<SettingField> otherFields)
    {
        fields.Clear();

        foreach (var f in otherFields)
        {
            SettingField fieldClone = f.Clone();
            List<SerializableEntry> entriesField = new();

            foreach (var e in fieldClone.entries)
            {
                SettingEntry entryClone = e.Clone();
                SerializableEntry entry = new SerializableEntry(entryClone.name, entryClone.type, entryClone.value.ToString());
                entriesField.Add(entry);

            }
            SerializableField fieldToAdd = new SerializableField(fieldClone.fieldName, entriesField);

            fields.Add(fieldToAdd);
        }
    }
    public SerializableGroupValuesXML Clone()
    {
        var flds = new List<SerializableField>();
        foreach (var field in fields)
        {
            flds.Add(field.Clone());
        }
        return new SerializableGroupValuesXML(flds);
    }

    public void ApplyTo(GroupValues target)
    {
        target.fields.Clear();
        foreach (var f in fields)
        {
            var ents = f.Clone();


            List<SettingEntry> fieldGV = new();

            foreach (var val in ents.entries)
            {
                var valuetoassign = val.Clone();
                SettingEntry sV = new();
                sV.name = val.key;
                sV.type = val.type;
                sV.ConvertStringToValue(val.value);

                fieldGV.Add(sV);

            }
            SettingField sFGV = new();
            sFGV.entries = fieldGV;
            target.fields.Add(sFGV);
        }
    }
}
[Serializable]
public class SerializableField
{
    public string name;
    public List<SerializableEntry> entries = new();
    public SerializableField() { }
    public SerializableField(string n, List<SerializableEntry> ents)
    {
        name = n;
        entries = ents;
    }
    public SerializableField Clone()
    {
        var ents = new List<SerializableEntry>();
        foreach (var value in entries)
        {
            ents.Add(value.Clone());
        }
        return new SerializableField(name, ents);
    }
}

[Serializable]
public class SerializableEntry
{
    public string key;
    public VALUE_TYPE type;
    public string value;
    public SerializableEntry() { }
    public SerializableEntry(string k, VALUE_TYPE t, string v)
    {
        key = k;
        type = t;
        value = v;
    }
    public SerializableEntry Clone()
    {
        return new SerializableEntry(key, type, (string)value.Clone());
    }
}
#endregion
#endregion