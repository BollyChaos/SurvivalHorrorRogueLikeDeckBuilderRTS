#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Xml.Serialization;

internal class GroupValuesImporterWindow : EditorWindow
{
    internal enum FileType { JSON, XML }
    FileType fileType;

    string selectedFilePath;

    [MenuItem("Tools/LoadSystem/Import GroupValues")]
    public static void Open()
    {
        GetWindow<GroupValuesImporterWindow>("Import GroupValues");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Import file", EditorStyles.boldLabel);

        SelectFileExtension();
        EditorGUILayout.Space();

        if (GUILayout.Button("Select File"))
        {
            string extension = fileType == FileType.JSON ? "json" : "xml";

            selectedFilePath = EditorUtility.OpenFilePanel(
                "Select GroupValues file",
                "",
                extension);
        }

        if (!string.IsNullOrEmpty(selectedFilePath))
        {
            EditorGUILayout.LabelField("File:", selectedFilePath);
        }

        EditorGUILayout.Space();

        GUI.enabled = !string.IsNullOrEmpty(selectedFilePath);

        if (GUILayout.Button("Import"))
        {
            ImportFile();
        }

        GUI.enabled = true;
    }

    void SelectFileExtension()
    {
        fileType = (FileType)GUILayout.Toolbar(
            (int)fileType,
            new string[] { "JSON", "XML" });
    }

    void ImportFile()
    {
        if (!File.Exists(selectedFilePath))
        {
            Debug.LogError("File not found");
            return;
        }


        GroupValues gv = ScriptableObject.CreateInstance<GroupValues>();
        
        if (fileType == FileType.JSON)
        {
            SerializableGroupSettings sgs = null;
            string json = File.ReadAllText(selectedFilePath);
            sgs = JsonUtility.FromJson<SerializableGroupSettings>(json);
            if (sgs == null)
            {
                Debug.LogError("Failed to deserialize file");
                return;
            }
            sgs.ApplyTo(gv);
        }
        else
        {
            SerializableGroupValuesXML sgs = null;
            XmlSerializer serializer =
                new XmlSerializer(typeof(SerializableGroupValuesXML));

            using FileStream stream = new FileStream(selectedFilePath, FileMode.Open);
            sgs = (SerializableGroupValuesXML)serializer.Deserialize(stream);
            if (sgs == null)
            {
                Debug.LogError("Failed to deserialize file");
                return;
            }
            sgs.ApplyTo(gv);

        }



        string assetPath = EditorUtility.SaveFilePanelInProject(
            "Save GroupValues Asset",
            "NewGroupValues",
            "asset",
            "Choose where to save the GroupValues asset");

        if (string.IsNullOrEmpty(assetPath))
            return;





        AssetDatabase.CreateAsset(gv, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = gv;

        Debug.Log("GroupValues imported successfully");
    }
}
#endif