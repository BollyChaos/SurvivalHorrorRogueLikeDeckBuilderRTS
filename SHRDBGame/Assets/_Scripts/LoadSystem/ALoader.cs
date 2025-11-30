using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ALoader : MonoBehaviour
{
    [Header("SO Path")]
    [SerializeField] protected string soPath = "Assets/Resources/LoadSystem/SavedFiles/";

    [SerializeField] protected string soName = "GroupValues.asset";

    [Header("JSON")]
    [SerializeField] protected string jsonFileName = "GameAssets.json";

    [SerializeField]
    protected GroupValues values;

    // ---------------------------------------------------------------------------------------
    // CARGAR
    // ---------------------------------------------------------------------------------------
    [ContextMenu("Cargar Datos")]
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



        // Cargar valores desde JSON
        LoadFromJsonFile();

        return values.Clone();
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
            Debug.LogWarning("El path no contiene 'Resources/'");
            return fullPath;
        }
    }

    // ---------------------------------------------------------------------------------------
    // GUARDAR
    // ---------------------------------------------------------------------------------------
    [ContextMenu("Guardar Datos")]
    public void SaveValues()
    {
        if (values == null) return;
        SaveToJsonFile();
    }

    public void SaveValues(GroupValues valuesToSave)
    {
        if (values.IsTheSame(valuesToSave))
        {
            Debug.Log("[Loader] El dato introducido es el mismo");
            return;
        }

        values = valuesToSave.Clone();
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
            Debug.LogWarning("[Loader] No existe JSON en: " + path);
            CreateJsonFile(path);
            return;
        }

        Debug.Log("[Loader] JSON encontrado en: " + path);

        string json = File.ReadAllText(path);
        SerializableGroupSettings sgs = new SerializableGroupSettings();
        JsonUtility.FromJsonOverwrite(json, sgs);

        sgs.ApplyTo(values);
    }

    // ---------------------------------------------------------------------------------------
    // JSON SAVE
    // ---------------------------------------------------------------------------------------
    protected virtual void SaveToJsonFile()
    {
        string path = GetJsonPath();

        SerializableGroupSettings sgs = new SerializableGroupSettings();
        sgs.CopyFrom(values);

        string json = JsonUtility.ToJson(sgs, true);
        File.WriteAllText(path, json);

        Debug.Log("[SettingsSerializer] Guardado en " + path);
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
            Debug.Log($"[Loader] El archivo JSON ya existe en: {fullPath}");
            return;
        }

        // Crear JSON por defecto con campos válidos
        SerializableGroupSettings defaults = new SerializableGroupSettings();
        defaults.CopyFrom(values);

        string json = JsonUtility.ToJson(defaults, true);
        File.WriteAllText(fullPath, json);

        Debug.Log($"[Loader] JSON creado en: {fullPath}");
    }
}

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

    public void ApplyTo(GroupValues target)
    {
        target.fields.Clear();
        foreach (var f in fields)
        {
            target.fields.Add(f.Clone());
        }
    }
}
