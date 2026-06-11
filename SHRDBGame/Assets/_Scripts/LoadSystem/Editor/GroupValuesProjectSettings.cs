#if UNITY_EDITOR
using System.Collections.Generic;

using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

[FilePath("ProjectSettings/GroupValuesProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
internal class GroupValuesProjectSettings : ScriptableSingleton<GroupValuesProjectSettings>
{
    public bool useTemplates;
    public EncryptionMethod encryptionMethod = EncryptionMethod.None;
    public string passwordSalt;
    void OnEnable()
    {
        
    }
    public static GroupValuesProjectSettings GetOrCreateSettings(string newPass = null)
    {
        var settings=instance;
        if (newPass == null)
        {


            if (string.IsNullOrEmpty(settings.passwordSalt))
            {
                settings.passwordSalt = PasswordGenerator.Generate(string.Empty);
                settings.Save(true);
            }
        }
        else
        {
            settings.passwordSalt=newPass;
            
        }

        return settings;
    }

    public void Save()
    {
        Save(true);
#if UNITY_EDITOR
        UpdateAllLoaders();
#endif
    }

#if UNITY_EDITOR
    void UpdateAllLoaders()
    {
        HashSet<Object> processedObjects = new HashSet<Object>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded)
                continue;
            foreach (GameObject root in scene.GetRootGameObjects())
            {

                foreach (var mb in root.GetComponentsInChildren<LoaderMono>(true))
                {
                    mb.ApplyEncryptionSettings(encryptionMethod, passwordSalt);
                    //RefreshInObject(mb, processedObjects);
                }
            }
        }

        EditorSceneManager.MarkAllScenesDirty();
    }
    private void RefreshInObject(Object obj, HashSet<Object> processed)
    {
        if (processed.Contains(obj))
            return;

        processed.Add(obj);
        SerializedObject serializedObject = new SerializedObject(obj);
        SerializedProperty prop = serializedObject.GetIterator();

        bool modified = false;

        while (prop.NextVisible(true))
        {
            if (prop.propertyType == SerializedPropertyType.Generic && prop.type == nameof(LoaderMono))
            {
                SerializedProperty loaderProp = prop.FindPropertyRelative("loader");
                SerializedProperty sceneAssetProp = prop.FindPropertyRelative("sceneAsset");

                if (loaderProp != null && sceneAssetProp.objectReferenceValue != null)
                {

                    string scenePath = AssetDatabase.GetAssetPath(sceneAssetProp.objectReferenceValue);
                    var em = loaderProp.FindPropertyRelative("encryptionMethod");
                    var pass = loaderProp.FindPropertyRelative("password");
                    if (em != null && pass != null)
                    {
                        em.enumValueFlag = (int)encryptionMethod;
                        pass.stringValue = passwordSalt;
                        modified = true;
                    }


                }
            }
        }
        if (modified)
            serializedObject.ApplyModifiedProperties();
    }
#endif
}
#endif