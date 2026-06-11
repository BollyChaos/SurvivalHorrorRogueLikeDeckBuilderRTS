#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Reflection;


internal class GroupValuesWindow : EditorWindow
{
    private enum EditorMode
    {
        GroupValues,
        Template
    }
    private EditorMode currentMode = EditorMode.GroupValues;
    //TODO: preparar para la encripcion y la copia a json
    ALoader loader = new ALoader();
    private List<GroupValues> allValues;
    private List<GroupValuesTemplate> allValuesT;
    private int selectedIndex = -1;
    private int selectedTemplateIndex = -1;
    private GroupValuesTemplate selectedTemplate;

    private GroupValuesTemplate workingCopyTemplate;
    //private GroupValuesTemplate originalCopyTemplate;

    private GroupValues selected;
    private GroupValues workingCopy;
    //private GroupValues originalCopy;

    private Vector2 scroll;

    [MenuItem("Tools/LoadSystem/Group Values Editor", priority = 0)]
    public static void Open()
    {
        GetWindow<GroupValuesWindow>("GroupValues");
    }

    void OnEnable()
    {
        loader.SetEncrytionSettings(GroupValuesProjectSettings.instance.encryptionMethod, GroupValuesProjectSettings.instance.passwordSalt);
        RefreshRegistry();
    }

    void RefreshRegistry()
    {
        allValues = GroupValuesRegistry.GetAll();
        allValuesT = GroupValuesRegistry.GetAllTemplates();
        Repaint();
    }

    void OnGUI()
    {
        DrawModeSelector();
        DrawProyectSettingsPanel();
        switch (currentMode)
        {
            case EditorMode.GroupValues:
                DrawRegistryPanel();

                if (selected != null)
                {
                    if (!showTemplateValues)
                        DrawToolbar();
                    DrawEditor();
                }
                break;

            case EditorMode.Template:
                DrawTemplatePanel();
                if (selectedTemplate != null)
                {
                    DrawTemplateToolbar();

                    DrawEditorTemplate();
                }
                break;
        }
    }
    //
    //MODE
    //
    void DrawModeSelector()
    {
        currentMode = (EditorMode)GUILayout.Toolbar(
            (int)currentMode,
            new string[] { "GroupValues", "Templates" });
        switch (currentMode)
        {
            case EditorMode.GroupValues:

                // selectedTemplate = null;
                // selectedTemplateIndex = -1;
                SelectTemplate(selectedTemplate);
                break;
            case EditorMode.Template:

                // selected = null;
                // selectedIndex = -1;
                Select(selected);
                break;
        }
    }
    // =========================================================
    // REGISTRY PANEL
    // =========================================================
    void DrawRegistryPanel()
    {
        EditorGUILayout.LabelField("GROUP VALUES REGISTRY", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh List"))
            RefreshRegistry();

        string[] names = allValues.ConvertAll(v => v.name).ToArray();

        int newIndex = EditorGUILayout.Popup("Selected Asset", selectedIndex, names);

        if (newIndex != selectedIndex && newIndex >= 0)
        {
            selectedIndex = newIndex;
            Select(allValues[selectedIndex]);

        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Create GroupValues"))
            CreateNewGroupValues();

        if (selected != null && GUILayout.Button("Delete Selected"))
            DeleteSelected();
        if (GUILayout.Button("Reset All"))
            ResetAllGroupValuesAndApply();

        EditorGUILayout.EndHorizontal();
        bool isReferenced = allValuesT.Exists(t => t.groupValuesReference == selected);

        // Solo mostrar toggle si es referencia de algún template

        if (isReferenced)
        {
            showTemplateValues = EditorGUILayout.ToggleLeft(
                "View Template Values",
                showTemplateValues
            );
        }
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
    void DrawTemplatePanel()
    {
        EditorGUILayout.LabelField("TEMPLATES REGISTRY", EditorStyles.boldLabel);

        if (GUILayout.Button("Refresh List"))
            RefreshRegistry();
        string[] names = allValuesT.ConvertAll(t => t.name).ToArray();
        int newIndex = EditorGUILayout.Popup("Selected Template", selectedTemplateIndex, names);
        if (newIndex != selectedTemplateIndex && newIndex >= 0)
        {
            selectedTemplateIndex = newIndex;
            SelectTemplate(allValuesT[selectedTemplateIndex]);
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Template"))
            CreateNewGroupValuesTemplate();


        if (selected != null && GUILayout.Button("Delete Selected"))
            DeleteSelectedTemplate();
        if (GUILayout.Button("Reset All"))
            ResetAllGroupValuesTemplatesAndApply();
        EditorGUILayout.EndHorizontal();
        if (allValuesT.Count == 0) return;



        if (selectedTemplate != null)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();

            var newRef = (GroupValues)EditorGUILayout.ObjectField(
                "Reference",
                workingCopyTemplate.groupValuesReference,
                typeof(GroupValues),
                false
            );

            if (EditorGUI.EndChangeCheck())
            {
                if (newRef != null && IsReferenceAlreadyUsed(newRef, workingCopyTemplate))
                {
                    EditorUtility.DisplayDialog(
                        "Reference already used",
                        "Another template is already using this GroupValues reference.",
                        "Ok"
                    );
                }
                else
                {
                    selectedTemplate.groupValuesReference = newRef;
                    workingCopyTemplate.groupValuesReference = newRef;

                    selectedTemplate.SetDefaultValuesInTemplate();
                    workingCopyTemplate.SetDefaultValuesInTemplate();

                    EditorUtility.SetDirty(workingCopyTemplate);
                }
            }
            if (GUILayout.Button("Template from Group Values"))
            {
                Undo.RecordObject(selectedTemplate, "Set default values in template");
                Undo.RecordObject(workingCopyTemplate, "Set default values in template");

                selectedTemplate.SetDefaultValuesInTemplate();
                workingCopyTemplate.SetDefaultValuesInTemplate();

                EditorUtility.SetDirty(selectedTemplate);
                EditorUtility.SetDirty(workingCopyTemplate);
                AssetDatabase.SaveAssets();
            }

            if (GUILayout.Button("Template to Group Values"))
            {
                Undo.RecordObject(selectedTemplate, "Set default values in template");
                Undo.RecordObject(workingCopyTemplate, "Set default values in template");

                selectedTemplate.SetDefaultValuesInSO();
                workingCopyTemplate.SetDefaultValuesInSO();

                EditorUtility.SetDirty(selectedTemplate);
                EditorUtility.SetDirty(workingCopyTemplate);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
    bool IsReferenceAlreadyUsed(GroupValues reference, GroupValuesTemplate currentTemplate)
    {
        var guids = AssetDatabase.FindAssets("t:GroupValuesTemplate");

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var template = AssetDatabase.LoadAssetAtPath<GroupValuesTemplate>(path);

            if (template == currentTemplate)
                continue;

            if (template.groupValuesReference == reference)
                return true;
        }

        return false;
    }
    void DrawTemplateToolbar()
    {
        if (selectedTemplate == null || selectedTemplate.groupValuesReference == null)
            return;

        GUILayout.BeginHorizontal();



        if (GUILayout.Button("Apply"))
        {
            ApplyTemplate();
        }

    

        if (GUILayout.Button("Reset To Defaults"))
        {
            Undo.RecordObject(selectedTemplate, "Reset to Defaults");
            Undo.RecordObject(workingCopyTemplate, "Reset to Defaults");

            selectedTemplate.SetDefaultValuesInSO();
            workingCopyTemplate = selectedTemplate.Clone();
           // originalCopyTemplate = selectedTemplate.Clone();
            Debug.Log("Reset template SO to defaults: " + selectedTemplate.groupValuesReference.name);

            EditorUtility.SetDirty(selectedTemplate);
            EditorUtility.SetDirty(workingCopyTemplate);
            AssetDatabase.SaveAssets();
        }

        GUILayout.EndHorizontal();
    }

    void DrawProyectSettingsPanel()
    {
        if (GUILayout.Button("Open Project Settings"))
        {
            SettingsService.OpenProjectSettings("Project/Load System");
        }
    }
    void ResetAllGroupValuesAndApply()
    {
        if (!EditorUtility.DisplayDialog(
            "Reset ALL GroupValues",
            "This will reset ALL GroupValues to defaults and overwrite JSON.\nThis cannot be undone.",
            "Yes", "Cancel"))
            return;

        var all = GroupValuesRegistry.GetAll();

        foreach (var gv in all)
        {
            gv.ResetToDefaults();
            EditorUtility.SetDirty(gv);

            // Rebuild JSON
            ApplyJsonForGroupValues(gv);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("All GroupValues reset and JSON regenerated");
    }
    void ResetAllGroupValuesTemplatesAndApply()
    {
        if (!EditorUtility.DisplayDialog(
            "Reset ALL GroupValuesTemplates",
            "This will reset ALL GroupValues to defaults as written in all templates.",
            "Yes", "Cancel"))
            return;

        var all = GroupValuesRegistry.GetAllTemplates();

        foreach (var gv in all)
        {
            gv.SetDefaultValuesInSO();
            EditorUtility.SetDirty(gv);


        }

        AssetDatabase.SaveAssets();
        Debug.Log("All GroupValues reset and JSON regenerated");
    }
    void ApplyJsonForGroupValues(GroupValues gv)
    {
        string assetPath = AssetDatabase.GetAssetPath(gv);
        string folder = Path.GetDirectoryName(assetPath);
        string name = Path.GetFileNameWithoutExtension(assetPath);

        loader.ChangeAssetName(name);

        //Hack: asignar el SO manualmente
        typeof(ALoader)
            .GetField("values", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(loader, gv);

        loader.SaveValues();
    }

    void Select(GroupValues gv)
    {
        if (gv != null)
        {
            selected = gv;
            //  originalCopy = gv.Clone();
            workingCopy = gv.Clone();
        }
    }
    void SelectTemplate(GroupValuesTemplate template)
    {
        if (template != null)
        {
            selectedTemplate = template;
            if (template.groupValuesReference != null)
            {
               // originalCopyTemplate = template.Clone();
                workingCopyTemplate = template.Clone();
            }
        }
    }
    // =========================================================
    // TOOLBAR
    // =========================================================
    void DrawToolbar()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Copy From Current"))
            DuplicateCurrentGroupValues();


        if (GUILayout.Button("Apply"))
            Apply();

        // if (GUILayout.Button("Undo"))
        //     Undo();

        if (GUILayout.Button("Reset To Defaults"))
        {
            Undo.RecordObject(workingCopy, "Reset to Defaults");
            workingCopy.ResetToDefaults();
            EditorUtility.SetDirty(workingCopy);
            AssetDatabase.SaveAssets();
        }

        GUILayout.EndHorizontal();
    }


    // =========================================================
    // EDITOR UI
    // =========================================================
    // void DrawEditor()
    // {
    //     scroll = EditorGUILayout.BeginScrollView(scroll);

    //     for (int i = 0; i < workingCopy.fields.Count; i++)
    //     {
    //         var field = workingCopy.fields[i];

    //         EditorGUILayout.BeginVertical("box");

    //         // Field header
    //         EditorGUILayout.BeginHorizontal();
    //         field.fieldName = EditorGUILayout.TextField("FIELD", field.fieldName);

    //         GUI.backgroundColor = Color.red;
    //         if (GUILayout.Button("X", GUILayout.Width(20)))
    //         {
    //             workingCopy.fields.RemoveAt(i);
    //             GUI.backgroundColor = Color.white;
    //             break;
    //         }
    //         GUI.backgroundColor = Color.white;
    //         EditorGUILayout.EndHorizontal();

    //         // Entries
    //         for (int j = 0; j < field.entries.Count; j++)
    //         {
    //             DrawEntry(field, j);
    //         }

    //         if (GUILayout.Button("+ Add Entry"))
    //         {
    //             AddEntry(field);
    //         }

    //         EditorGUILayout.EndVertical();
    //         EditorGUILayout.Space();
    //     }

    //     if (GUILayout.Button("+ Add Field"))
    //     {
    //         workingCopy.fields.Add(new SettingField()
    //         {
    //             fieldName = "NewField"
    //         });
    //     }

    //     EditorGUILayout.EndScrollView();
    // }
    bool showTemplateValues = false;
    void DrawEditor()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);



        //var displayCopy = showTemplateValues ? GetTemplateForSelected(selected)?.Clone() : workingCopyTemplate;
        List<SettingField> fieldsDisplay = showTemplateValues ? GetTemplateForSelected(selected)?.Clone().fields : workingCopy.fields;
        for (int i = 0; i < fieldsDisplay.Count; i++)
        {
            var field = fieldsDisplay[i];

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            field.fieldName = EditorGUILayout.TextField("Field Name", field.fieldName);

            GUI.backgroundColor = Color.red;
            if (!showTemplateValues && GUILayout.Button("X", GUILayout.Width(20)))
            {
                Undo.RecordObject(workingCopy, "Delete Entry");
                workingCopy.fields.RemoveAt(i);
                EditorUtility.SetDirty(workingCopy);
                AssetDatabase.SaveAssets();
                GUI.backgroundColor = Color.white;
                break;
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            // Entries
            for (int j = 0; j < field.entries.Count; j++)
            {
                DrawEntry(field, j, showTemplateValues); // adaptado para deshabilitar edición si es template
            }

            if (!showTemplateValues && GUILayout.Button("+ Add Entry"))
            {
                AddEntry(field);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (!showTemplateValues && GUILayout.Button("+ Add Field"))
        {
            Undo.RecordObject(workingCopy, "Add field");
            workingCopy.fields.Add(new SettingField() { fieldName = "NewField" });
            EditorUtility.SetDirty(workingCopy);
            AssetDatabase.SaveAssets();

        }

        EditorGUILayout.EndScrollView();
    }

    // Devuelve el template que referencia al GroupValues seleccionado
    GroupValuesTemplate GetTemplateForSelected(GroupValues gv)
    {
        return allValuesT.Find(t => t.groupValuesReference == gv);
    }

    // Adaptar DrawEntry para que solo sea editable si no estás viendo los valores del template
    void DrawEntry(SettingField field, int index, bool readOnly)
    {
        var entry = field.entries[index];
        EditorGUILayout.BeginHorizontal();

        EditorGUI.BeginDisabledGroup(readOnly);
        entry.name = EditorGUILayout.TextField(entry.name, GUILayout.Width(150));

        var newType = (VALUE_TYPE)EditorGUILayout.EnumPopup(entry.type, GUILayout.Width(80));
        if (newType != entry.type)
        {
            entry.type = newType;
            entry.value = SettingValueFactory.Create(newType);
        }

        DrawEntryValue(entry);
        if (!readOnly)
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                Undo.RecordObject(workingCopy, "Delete Entry");
                field.entries.RemoveAt(index);
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
                EditorUtility.SetDirty(workingCopy);
                AssetDatabase.SaveAssets();
                return;
            }
            GUI.backgroundColor = Color.white;
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();
    }
    void DrawEditorTemplate()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        for (int i = 0; i < workingCopyTemplate.fields.Count; i++)
        {
            var field = workingCopyTemplate.fields[i];

            EditorGUILayout.BeginVertical("box");

            // Field header
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(field.fieldName, GUILayout.Width(150));

            GUI.backgroundColor = Color.red;

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            // Entries
            for (int j = 0; j < field.entries.Count; j++)
            {
                DrawEntryTemplate(field, j);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }


        EditorGUILayout.EndScrollView();
    }

    // void DrawEntry(SettingField field, int index)
    // {
    //     var entry = field.entries[index];
    //     if (field.entries.Exists(x => x != entry && x.name == entry.name))
    //         EditorGUILayout.HelpBox("Duplicate key!", MessageType.Error);

    //     Color originalColor = GUI.backgroundColor;
    //     GUI.backgroundColor = (index % 2 == 0) ? originalColor : originalColor * 0.65f;

    //     EditorGUILayout.BeginHorizontal();

    //     entry.name = EditorGUILayout.TextField(entry.name, GUILayout.Width(150));

    //     // Type selector
    //     var newType = (VALUE_TYPE)EditorGUILayout.EnumPopup(entry.type, GUILayout.Width(80));
    //     if (newType != entry.type)
    //     {
    //         entry.type = newType;
    //         entry.value = SettingValueFactory.Create(newType); // recreate value
    //     }

    //     EditorGUILayout.Space(20);
    //     DrawEntryValue(entry);

    //     GUI.backgroundColor = Color.red;
    //     if (GUILayout.Button("X", GUILayout.Width(20)))
    //     {
    //         field.entries.RemoveAt(index);
    //         GUI.backgroundColor = originalColor;
    //         EditorGUILayout.EndHorizontal();
    //         return;
    //     }

    //     GUI.backgroundColor = originalColor; // restaurar el color
    //     EditorGUILayout.EndHorizontal();
    // }
    void DrawEntryTemplate(SettingField field, int index)
    {
        var entry = field.entries[index];
        if (field.entries.Exists(x => x != entry && x.name == entry.name))
            EditorGUILayout.HelpBox("Duplicate key!", MessageType.Error);


        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = (index % 2 == 0) ? originalColor : originalColor * 0.65f;

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(entry.name, GUILayout.Width(150));

        // Type selector
        var newType = (VALUE_TYPE)EditorGUILayout.EnumPopup(entry.type, GUILayout.Width(80));
        if (newType != entry.type)
        {
            entry.type = newType;
            entry.value = SettingValueFactory.Create(newType); // recreate value
        }
        EditorGUILayout.Space(20);
        DrawEntryValue(entry);


        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndHorizontal();
    }
    void DrawEntryValue(SettingEntry entry)
    {
        switch (entry.type)
        {
            case VALUE_TYPE.BOOL:
                ((BoolSettingValue)entry.value).value =
                    EditorGUILayout.Toggle(((BoolSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.INT:
                ((IntSettingValue)entry.value).value =
                    EditorGUILayout.IntField(((IntSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.FLOAT:
                ((FloatSettingValue)entry.value).value =
                    EditorGUILayout.FloatField(((FloatSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.DOUBLE:
                ((DoubleSettingValue)entry.value).value =
                    EditorGUILayout.DoubleField(((DoubleSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.LONG:
                ((LongSettingValue)entry.value).value =
                    EditorGUILayout.LongField(((LongSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.SHORT:
                ((ShortSettingValue)entry.value).value =
                    (short)EditorGUILayout.IntField(((ShortSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.BYTE:
                ((ByteSettingValue)entry.value).value =
                    (byte)EditorGUILayout.IntField(((ByteSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.STRING:
                ((StringSettingValue)entry.value).value =
                    EditorGUILayout.TextField(((StringSettingValue)entry.value).value);
                break;

            case VALUE_TYPE.VECTOR2:
                ((Vector2SettingValue)entry.value).value =
                    EditorGUILayout.Vector2Field("", ((Vector2SettingValue)entry.value).value);
                break;
        }
    }

    void AddEntry(SettingField field)
    {
        Undo.RecordObject(workingCopy, "Add Entry");
        var e = new SettingEntry();
        e.name = "NewEntry_" + field.entries.Count;

        e.type = VALUE_TYPE.INT;
        e.value = SettingValueFactory.Create(e.type);

        field.entries.Add(e);

        EditorUtility.SetDirty(workingCopy);
        AssetDatabase.SaveAssets();
        //Apply();


    }


    // =========================================================
    // APPLY / UNDO
    // =========================================================
    void DuplicateCurrentGroupValues()
    {
        if (selected == null)
            return;

        string path = AssetDatabase.GetAssetPath(selected);
        string newPath = AssetDatabase.GenerateUniqueAssetPath(path.Replace(".asset", "_Copy.asset"));

        GroupValues copy = ScriptableObject.CreateInstance<GroupValues>();
        copy.CopyFrom(selected);

        AssetDatabase.CreateAsset(copy, newPath);
        AssetDatabase.SaveAssets();

        Debug.Log("GroupValues duplicated: " + newPath);
        selected = copy;
    }

    void Apply()
    {
        selected.CopyFrom(workingCopy);

        EditorUtility.SetDirty(selected);
        AssetDatabase.SaveAssets();

        // Guardar JSON con ALoader
        SaveJsonForAsset(selected);

        //originalCopy = selected.Clone();
        //Debug.Log("Applied changes to:" + selected.name + " + JSON updated");
    }
    void ApplyTemplate()
    {
        selectedTemplate.CopyFrom(workingCopyTemplate);

        EditorUtility.SetDirty(selectedTemplate);
        AssetDatabase.SaveAssets();

       // originalCopyTemplate = selectedTemplate.Clone();
    }

    void SaveJsonForAsset(GroupValues asset)
    {
        loader.ChangeAssetName(asset.name);

        loader.AutoResolveFromResources();//already has path
                                          //        Debug.Log(asset.name);
        loader.SaveValues(asset);
    }


    // void Undo()
    // {
    //    // workingCopy = originalCopy.Clone();
    //     Debug.Log("Undo changes");
    // }

    // =========================================================
    // CREATE / DELETE
    // =========================================================
    void CreateNewGroupValues()
    {

        string path = EditorUtility.SaveFilePanelInProject(
            "Create GroupValues",
            "NewGroupValues",
            "asset",
            "Save GroupValues"
        );


        if (string.IsNullOrEmpty(path))
            return;

        var gv = ScriptableObject.CreateInstance<GroupValues>();
        AssetDatabase.CreateAsset(gv, path);
        AssetDatabase.SaveAssets();

        RefreshRegistry();
    }
    void CreateNewGroupValuesTemplate()
    {
        string path = EditorUtility.SaveFilePanelInProject(
          "Create GroupValuesTemplate",
          "NewGroupValuesTemplate",
          "asset",
          "Save GroupValuesTempalte"
      );


        if (string.IsNullOrEmpty(path))
            return;

        var gvT = ScriptableObject.CreateInstance<GroupValuesTemplate>();
        AssetDatabase.CreateAsset(gvT, path);
        AssetDatabase.SaveAssets();

        RefreshRegistry();
    }
    void DeleteSelected()
    {
        if (!EditorUtility.DisplayDialog("Delete GroupValues?",
            $"Delete {selected.name}?\nThis cannot be undone.",
            "Yes", "Cancel"))
            return;

        string path = AssetDatabase.GetAssetPath(selected);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();

        selected = null;
        workingCopy = null;
        // originalCopy = null;
        selectedIndex = -1;

        RefreshRegistry();
    }
    void DeleteSelectedTemplate()
    {
        if (!EditorUtility.DisplayDialog("Delete GroupValues?",
           $"Delete {selected.name}?\nThis cannot be undone.",
           "Yes", "Cancel"))
            return;

        string path = AssetDatabase.GetAssetPath(selectedTemplate);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();

        selectedTemplate = null;
        workingCopyTemplate = null;
        selectedTemplateIndex = -1;

        RefreshRegistry();
    }
}
#endif
