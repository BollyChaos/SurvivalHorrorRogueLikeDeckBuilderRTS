#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

static class GroupValuesSettingsProvider
{
    [SettingsProvider]
    public static SettingsProvider CreateProvider()
    {
        var provider = new SettingsProvider(
            "Project/Load System",
            SettingsScope.Project)
        {
            label = "Group Values",
            guiHandler = (searchContext) =>
            {
                var settings = GroupValuesProjectSettings.instance;
                SerializedObject so = new SerializedObject(settings);
                so.Update();

                EditorGUI.BeginChangeCheck();


                bool useTemplates = EditorGUILayout.Toggle(
                "Use Templates",
                settings.useTemplates
            );



                var encryptionMethod = (EncryptionMethod)EditorGUILayout.EnumPopup(
                "Encryption Method",
                settings.encryptionMethod);

                if (EditorGUI.EndChangeCheck())
                {
                    settings.encryptionMethod=encryptionMethod;
                    settings.useTemplates = useTemplates;
                    settings.Save();
                }

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField(
                    "Password Salt",
                    settings.passwordSalt
                );

                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Generate New Salt"))
                {
                    settings.passwordSalt = PasswordGenerator.GenerateNewPassword("");
                    GroupValuesProjectSettings.GetOrCreateSettings();
                    settings.Save();
                }


            }
        };

        return provider;
    }
}
#endif