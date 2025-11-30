using UnityEngine;

public class WebLoader : ALoader
{
    private const string WEB_KEY = "web_group_values";

    // ----------------------------------------------------------------------
    // JSON LOAD override → lee desde PlayerPrefs
    // ----------------------------------------------------------------------
    protected override void LoadFromJsonFile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!PlayerPrefs.HasKey(WEB_KEY))
        {
            Debug.Log("[WebLoader] No existe JSON, creando valores por defecto.");
            CreateJsonForWeb();
            return;
        }

        string json = PlayerPrefs.GetString(WEB_KEY);
        SerializableGroupSettings sgs = new SerializableGroupSettings();
        JsonUtility.FromJsonOverwrite(json, sgs);
        sgs.ApplyTo(values);

        Debug.Log("[WebLoader] JSON cargado desde PlayerPrefs");
#else
        base.LoadFromJsonFile();
#endif
    }

    // ----------------------------------------------------------------------
    // JSON SAVE override → guarda en PlayerPrefs
    // ----------------------------------------------------------------------
    protected override void SaveToJsonFile()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SerializableGroupSettings sgs = new SerializableGroupSettings();
        sgs.CopyFrom(values);

        string json = JsonUtility.ToJson(sgs, true);

        PlayerPrefs.SetString(WEB_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("[WebLoader] JSON guardado en PlayerPrefs");
#else
        base.SaveToJsonFile();
#endif
    }

    // ----------------------------------------------------------------------
    // Crea datos iniciales en WebGL
    // ----------------------------------------------------------------------
   private void CreateJsonForWeb()
{
    if (values == null)
    {
        Debug.LogError("[WebLoader] 'values' es null, no se pueden crear datos iniciales.");
        return;
    }

    SerializableGroupSettings sgs = new SerializableGroupSettings();
    sgs.CopyFrom(values);

    string json = JsonUtility.ToJson(sgs, true);
    PlayerPrefs.SetString(WEB_KEY, json);
    PlayerPrefs.Save();

    Debug.Log("[WebLoader] JSON WEB creado");
}

}
