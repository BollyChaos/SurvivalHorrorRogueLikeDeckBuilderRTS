#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GroupValuesDocumentationWindow : EditorWindow
{
    private Vector2 indexScroll;
    private Vector2 contentScroll;

    private GroupValues documentationGV;

    private int selectedIndex = -1;
    void OnEnable()
    {
        documentationGV = GroupValuesRegistry.FindByName("Documentation");
    }

    [MenuItem("Help/LoadSystem Documentation", priority = 101)]
    public static void Open()
    {
        GetWindow<GroupValuesDocumentationWindow>("Documentation");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("GroupValues Documentation", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Select GroupValues ScriptableObject
        documentationGV = (GroupValues)EditorGUILayout.ObjectField(
            "Documentation Source",
            documentationGV,
            typeof(GroupValues),
            false
        );

        if (documentationGV == null) return;

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        DrawIndexPanel();
        DrawContentPanel();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawIndexPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        EditorGUILayout.LabelField("Topics", EditorStyles.boldLabel);

        indexScroll = EditorGUILayout.BeginScrollView(indexScroll);

        GUIStyle topicStyle = new GUIStyle(GUI.skin.box)
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
            padding = new RectOffset(6, 6, 6, 6)
        };

        for (int i = 0; i < documentationGV.fields.Count; i++)
        {
            var field = documentationGV.fields[i];

            if (i == selectedIndex)
                GUI.backgroundColor = new Color(0.6f, 0.6f, 0.6f);

            if (GUILayout.Button(field.fieldName, topicStyle, GUILayout.ExpandWidth(true)))
            {
                selectedIndex = i;
            }

            GUI.backgroundColor = Color.white;
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawContentPanel()
    {
        EditorGUILayout.BeginVertical();

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            richText = true,
            wordWrap = true,
            fontSize = 16
        };

        GUIStyle textStyle = new GUIStyle(EditorStyles.label)
        {
            richText = true,
            wordWrap = true,
            fontSize = 14
        };

        EditorGUILayout.LabelField("Content", titleStyle);
        contentScroll = EditorGUILayout.BeginScrollView(contentScroll);

        if (selectedIndex >= 0 && selectedIndex < documentationGV.fields.Count)
        {
            var field = documentationGV.fields[selectedIndex];
            foreach (var entry in field.entries)
            {
                EditorGUILayout.LabelField(entry.name, titleStyle);
                EditorGUILayout.LabelField(entry.value.GetValue().ToString(), textStyle);
                EditorGUILayout.Space();
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }
}
#endif