#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
internal class GroupValuesBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    static ALoader loader = new ALoader();
    static Dictionary<string, GroupValues> GetGroupValues()
    {
        var dict = new Dictionary<string, GroupValues>();

        var all = GroupValuesRegistry.GetAll();

        foreach (var gv in all)
        {

            dict[gv.name] = gv;
        }

        return dict;
    }
    static Dictionary<string, GroupValuesTemplate> GetTemplates()
    {
        var dict = new Dictionary<string, GroupValuesTemplate>();

        var all = GroupValuesRegistry.GetAllTemplates();

        foreach (var gv in all)
        {

            dict[gv.name] = gv;
        }

        return dict;
    }

    /*
    STEPS BEFORE MAKING BUILD
    1. CHECK THAT EACH LOADERMONO COMPNT HAS THE RIGHT ENCRYPMTH AND PASS
    2. ERASE ALL DATA FROM JSON OR EVEN PLAYER PREFS(DONT STORE PROYECT RELATED VALUES MADE ON TESTING)
    3. GO FIRST TO CHECK TEMPLATES, STORE THEM ON A LIST
    4. SET DEFAULT VALUES FROM TEMPLATES TO ITS CORRESPONDING REFERENCE
    5. GET ALL GROUP VALUES THAT ARE NOT REFERENCES FROM ANOTHER TEMPLATE AND SET THEM TO DEFAULT VALUES
    */
    public void OnPreprocessBuild(BuildReport report)
    {

        Debug.Log("[GroupValuesBuildProcessor] build preprocess started");

        ApplyEncryptionSettings();

        ClearTestingData();

        var templates = GroupValuesRegistry.GetAllTemplates();

        ApplyTemplateDefaults(templates);

        ResetNonTemplateGroupValues(templates);

        Debug.Log("[GroupValuesBuildProcessor] build preprocess finished");


    }



    void ApplyEncryptionSettings()
    {
        loader.SetEncrytionSettings(GroupValuesProjectSettings.instance.encryptionMethod, GroupValuesProjectSettings.instance.passwordSalt);

        var settings = GroupValuesProjectSettings.instance;

        var loaders = Object.FindObjectsOfType<LoaderMono>(true);

        foreach (var loader in loaders)
        {
            loader.ApplyEncryptionSettings(
                settings.encryptionMethod,
                settings.passwordSalt
            );

            EditorUtility.SetDirty(loader);
        }
    }
    void ClearTestingData()
    {
        PlayerPrefs.DeleteAll();

    }
    void ApplyTemplateDefaults(List<GroupValuesTemplate> templates)
    {

        foreach (var template in templates)
        {
            if (template.groupValuesReference == null)
            {

                throw new BuildFailedException("[GroupValuesBuildProcessor]Template without reference detected");

            }
           Debug.Log(template.fields[0].entries[0].value);

            loader.RemoveLoadedValues();


            loader.ChangeAssetName(template.groupValuesReference.name);

            loader.AutoResolveFromResources();



            template.SetDefaultValuesInSO();
            //save in persistentdata, because that is what the player will have stored
         
            SerializableGroupSettings sgs=new ();
            if(template.fields==null)throw new BuildFailedException("[GroupValuesBuildProcessor]Template without fields detected");
            sgs.CopyFrom(template.fields);


            loader.SaveValuesInPersistentData(sgs);
            loader.SaveValues(sgs);

            EditorUtility.SetDirty(template.groupValuesReference);
        }
    }
    private void ResetNonTemplateGroupValues(List<GroupValuesTemplate> templates)
    {

        var referenced = new HashSet<string>();

        foreach (var template in templates)
        {
            if (template.groupValuesReference != null)
                referenced.Add(template.groupValuesReference.name);
        }

        var groupValues = GroupValuesRegistry.GetAll();
        foreach (var gv in groupValues)
        {

            if (gv == null)
                throw new BuildFailedException("[GroupValuesBuildProcessor]GroupValue is null");

            if (referenced.Contains(gv.name))//Already has set values
                continue;

            gv.ResetToDefaults();
            loader.RemoveLoadedValues();
            loader.SetGroupValues(gv);
            loader.ChangeAssetName(gv.name);
            loader.AutoResolveFromResources();

            //save in persistentdata, because that is what the player will have stored
            loader.SaveValuesInPersistentData();
            loader.SaveValues();

            EditorUtility.SetDirty(gv);
        }
    }

}
#endif
