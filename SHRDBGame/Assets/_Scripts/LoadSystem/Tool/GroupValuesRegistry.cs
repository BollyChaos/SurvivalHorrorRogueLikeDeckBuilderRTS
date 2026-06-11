#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


public static class GroupValuesRegistry
{
    static readonly HashSet<string> reservedFileNames = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase) { "Documentation" };
    public static List<GroupValues> GetAll()
    {
        var guids = AssetDatabase.FindAssets("t:GroupValues");
        var list = new List<GroupValues>();


        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            var fileName = System.IO.Path.GetFileNameWithoutExtension(path);

            if (reservedFileNames.Contains(fileName))
                continue;


            list.Add(AssetDatabase.LoadAssetAtPath<GroupValues>(path));
        }
        return list;
    }
    public static List<GroupValuesTemplate> GetAllTemplates()
    {
        var guids = AssetDatabase.FindAssets("t:GroupValuesTemplate");
        var list = new List<GroupValuesTemplate>();


        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
            list.Add(AssetDatabase.LoadAssetAtPath<GroupValuesTemplate>(path));
        }
        return list;
    }
    public static GroupValues FindByName(string name)
    {
        var guids = AssetDatabase.FindAssets($"{name} t:GroupValues");

        if (guids.Length == 0)
            return null;

        var path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return AssetDatabase.LoadAssetAtPath<GroupValues>(path);
    }
}
#endif