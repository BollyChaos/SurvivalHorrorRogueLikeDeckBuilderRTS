using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ALoader : MonoBehaviour
{
    //Nombre del path del so que contiene los datos
    //Mirar ejemplo escrito
    [SerializeField]
    [Tooltip("Path of the SO asset")]
    protected string soPath = "Assets/_Scripts/LoadSystem/SavedFiles/";
    [SerializeField]
    [Tooltip("SO name in the directory")]
    protected string soName = "GroupValues.asset";

    [Tooltip("Path of the JSON file")]
    [SerializeField]
    protected string jsonPath="Assets/_Scripts/LoadSystem/SavedFiles/";
    //por ejemplo="GameSettings.json"
    [SerializeField]
    [Tooltip("JSON name in the directory")]
    protected string jsonFileName="GameAssets.json";

    [SerializeField]
    [Tooltip("SO que almacena los datos, se inicializa al cargar los datos")]
    protected GroupValues values;

    [ContextMenu("Cargar Datos")]
  [ContextMenu("Cargar Datos")]
public GroupValues LoadValues()
{
    if (values == null)
    {
#if UNITY_EDITOR
        if (!File.Exists(soPath + soName))
        {
            Debug.LogWarning("No existe el archivo en: " + soPath + soName);
        }
        values = AssetDatabase.LoadAssetAtPath<GroupValues>(soPath + soName);
#else
        // En la build, se intenta cargar desde Resources (si existe ahí)
        string resourcePath = Path.GetFileNameWithoutExtension(soName);
        values = Resources.Load<GroupValues>(resourcePath);
#endif

        LoadFromJsonFile();
    }
    return values.Clone();
}


    [ContextMenu("Guardar Datos")]
    public void SaveValues()
    {
        if (values == null) return;

        SaveToJsonFile();
    }
    [ContextMenu("Sacar Datos")]
    public void GetData()
    {
        Debug.Log(values.GetValue<float>("Contrast"));
        Debug.Log(values.GetValue<float>("MasterVolume"));
    }
    public void SaveValues(GroupValues valuesToSave)
    {

        if(values.IsTheSame(valuesToSave)){ Debug.Log("[Loader] El dato introducido es el mismo"); return; }
        values = valuesToSave.Clone();
        if (values == null) return;

        SaveToJsonFile();
        
    }
    void LoadFromJsonFile()
    {
        if (!File.Exists(GetJsonPath()))
        {
            Debug.LogWarning("[Loader]No se encontro JSON en:" + GetJsonPath());
            Debug.LogWarning("Se va a crear el archivo en:" + jsonPath);
            CreateJsonFile(jsonPath, jsonFileName);
            return;
        }
        Debug.Log("[Loader]Se encontro JSON en:"+GetJsonPath());
        string jsonData= File.ReadAllText(GetJsonPath());
        SerializableGroupSettings sgs = new SerializableGroupSettings();
        JsonUtility.FromJsonOverwrite(jsonData, sgs);
        sgs.ApplyTo(values);
    }

    void SaveToJsonFile()
    {
        SerializableGroupSettings sgs = new SerializableGroupSettings();
        sgs.CopyFrom(values);
        SettingsSerializer.SaveToJson(sgs, GetJsonPath());
    }
    private string GetJsonPath()
    {
#if UNITY_EDITOR
        return jsonPath+jsonFileName;
#else
        return Path.Combine(Application.persistentDataPath, jsonFileName);
#endif
    }
    public void CreateJsonFile(string folderPath, string fileName)
    {
        // Asegura que la carpeta exista
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // Crea el path completo (añade .json si falta)
        string fullPath = Path.Combine(folderPath, fileName);
        if (!fullPath.EndsWith(".json")) fullPath += ".json";

        // Si ya existe, no lo sobrescribe (opcional)
        if (File.Exists(fullPath))
        {
            Debug.Log($"El archivo JSON ya existe en: {fullPath}");
            return;
        }

        // Escribe un JSON vacío
        File.WriteAllText(fullPath, "{}");
        Debug.Log($"Archivo JSON vacío creado en: {fullPath}");
    }
}
public class SerializableGroupSettings
{
    public List<SettingField> fields = new();

    public void CopyFrom(GroupValues settings)
    {
        fields.Clear();
        foreach (var field in settings.fields)
        {
            fields.Add(field.Clone());
        }
    }

    public void ApplyTo(GroupValues target)
    {
        target.fields.Clear();
        foreach (var field in fields)
        {
            target.fields.Add(field.Clone());
        }
    }
}

public static class SettingsSerializer
{
    public static void SaveToJson<T>(T data, string path)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log("[SettingsSerializer] Guardado en " + path);
    }

    public static void LoadFromJson<T>(T target, string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning("[SettingsSerializer] Archivo no encontrado: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        JsonUtility.FromJsonOverwrite(json, target);
        Debug.Log("[SettingsSerializer] Cargado desde " + path);
    }
}
