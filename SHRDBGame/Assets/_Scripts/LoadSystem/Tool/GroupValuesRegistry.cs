#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


public static class GroupValuesRegistry
{
    
    public static List<GroupValues> GetAll()
    {
        var guids = AssetDatabase.FindAssets("t:GroupValues");
        var list = new List<GroupValues>();


        foreach (var g in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(g);
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
}
#endif