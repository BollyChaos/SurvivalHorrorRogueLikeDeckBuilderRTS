#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

internal class GroupValuesExporterWindow : EditorWindow
{
    internal enum FileType { JSON, XML }

    FileType fileType;
    GroupValues gvToExport;

    [MenuItem("Tools/LoadSystem/Export GroupValues")]
    public static void Open()
    {
        GetWindow<GroupValuesExporterWindow>("Export GroupValues");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Export GroupValues", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        gvToExport = (GroupValues)EditorGUILayout.ObjectField(
            "GroupValues",
            gvToExport,
            typeof(GroupValues),
            false
        );

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("File Type");
        SelectFileExtension();

        EditorGUILayout.Space();

        GUI.enabled = gvToExport != null;

        if (GUILayout.Button("Export"))
        {
            Export();
        }

        GUI.enabled = true;
    }

    void SelectFileExtension()
    {
        fileType = (FileType)GUILayout.Toolbar(
            (int)fileType,
            new string[] { "JSON", "XML" });
    }

    void Export()
    {
        string extension = fileType == FileType.JSON ? "json" : "xml";

        string path = EditorUtility.SaveFilePanel(
            "Export GroupValues",
            Application.dataPath,
            gvToExport.name,
            extension
        );

        if (string.IsNullOrEmpty(path))
            return;

        string content = "";

        if (fileType == FileType.JSON)
        {
            var sgs = new SerializableGroupSettings();
            sgs.CopyFrom(gvToExport);
            content = JsonUtility.ToJson(sgs, true);
            File.WriteAllText(path, content);
        }
        else
        {

            content = ConvertToXML(gvToExport);
            File.WriteAllText(path,content,System.Text.Encoding.Unicode);
            
        }

        

        EditorUtility.RevealInFinder(path);
        Debug.Log($"GroupValues exported to: {path}");
    }

    string ConvertToXML(GroupValues gv)
    {
        var sgs = new SerializableGroupValuesXML();
        sgs.CopyFrom(gv);

        System.Xml.Serialization.XmlSerializer serializer =
            new System.Xml.Serialization.XmlSerializer(typeof(SerializableGroupValuesXML));

        using (StringWriter writer = new StringWriter())
        {
            serializer.Serialize(writer, sgs);
            return writer.ToString();
        }
        
    }
}
#endif