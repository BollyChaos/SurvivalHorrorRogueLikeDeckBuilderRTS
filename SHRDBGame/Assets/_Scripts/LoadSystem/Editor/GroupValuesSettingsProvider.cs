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

                EditorGUI.BeginChangeCheck();

                settings.encryptionMethod = (EncryptionMethod)EditorGUILayout.EnumPopup(
    "Encryption Method",
    settings.encryptionMethod);

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

                if (EditorGUI.EndChangeCheck())
                {
                    settings.Save();
                }
            }
        };

        return provider;
    }
}
#endif