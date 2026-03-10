#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
static class GroupValuesSettingsSync
{
    static GroupValuesSettingsSync()
    {
        SyncToRuntimeAsset();
    }

    static void SyncToRuntimeAsset()
    {
        var editorSettings = GroupValuesProjectSettings.instance;

        var runtimeAsset = Resources.Load<GroupValuesRuntimeSettings>("GroupValuesRuntimeSettings");

        if (runtimeAsset == null)
            return;

        runtimeAsset.encryptionMethod = editorSettings.encryptionMethod;
        runtimeAsset.passwordSalt = editorSettings.passwordSalt;

        EditorUtility.SetDirty(runtimeAsset);
        AssetDatabase.SaveAssets();
    }
}
#endif