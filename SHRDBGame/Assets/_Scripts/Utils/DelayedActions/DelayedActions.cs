using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class DelayedActions
{

    static DelayedActionsInScene delayedActionsInScene;

    #region Public Methods
    public static int Do(Action action, float delay, MonoBehaviour executor, string actionName = "Action")
    {
        CheckDelayedActionsInScene();

        DelayedActionInfo info = new DelayedActionInfo
        {
            action = action,
            delay = delay,
#if UNITY_EDITOR
            actionName = actionName
#endif
        };
        delayedActionsInScene.AddAction(info, executor);
        return info.id;

    }
    public static int Do(Action action, float delay, MonoBehaviour executor, int ID, string actionName = "Action")
    {
        CheckDelayedActionsInScene();

        DelayedActionInfo info = new DelayedActionInfo
        {
            action = action,
            delay = delay,
#if UNITY_EDITOR
            actionName = actionName,
#endif
            id=ID
        };
        info.id = ID;
        delayedActionsInScene.AddAction(info, executor);
        return info.id;

    }

    public static void Abort(MonoBehaviour executor)
    {
        if (delayedActionsInScene == null)
            return;
        delayedActionsInScene.Abort(executor);
    }
    public static void Abort(MonoBehaviour executor, int ID)
    {
        if (delayedActionsInScene == null)
            return;
        delayedActionsInScene.Abort(executor, ID);
    }
    public static void Abort(int ID)
    {
        if (delayedActionsInScene == null)
            return;
        delayedActionsInScene.Abort(ID);
    }
    #endregion

    #region InternalWorking
    private static void CheckDelayedActionsInScene()
    {
        if (delayedActionsInScene == null)
        {
            GameObject go = new GameObject("DelayedActions");
            delayedActionsInScene = go.AddComponent<DelayedActionsInScene>();
        }
    }
    #endregion

    #region TestingCode
    #if UNITY_EDITOR

    static DelayedActionsInScene delayedActionsInSceneB;

    public static int DoB(Action action, float delay, MonoBehaviour executor, string actionName = "Action")
    {
        CheckDelayedActionsInSceneB();

        DelayedActionInfo info = new DelayedActionInfo
        {
            action = action,
            delay = delay,

            actionName = actionName

        };
        delayedActionsInSceneB.AddAction(info, executor);
        return info.id;

    }
    public static int DoB(Action action, float delay, MonoBehaviour executor, int ID, string actionName = "Action")
    {
        CheckDelayedActionsInSceneB();

        DelayedActionInfo info = new DelayedActionInfo
        {
            action = action,
            delay = delay,

            actionName = actionName,
            id = ID
        };
        info.id = ID;

        delayedActionsInSceneB.AddAction(info, executor);
        return info.id;

    }

    public static void AbortB(MonoBehaviour executor)
    {
        if (delayedActionsInSceneB == null)
            return;
        delayedActionsInSceneB.Abort(executor);
    }
    public static void AbortB(MonoBehaviour executor, int ID)
    {
        if (delayedActionsInSceneB == null)
            return;
        delayedActionsInSceneB.Abort(executor, ID);
    }
    public static void AbortB(int ID)
    {
        if (delayedActionsInSceneB == null)
            return;
        delayedActionsInSceneB.Abort(ID);
    }
    private static void CheckDelayedActionsInSceneB()
    {
        if (delayedActionsInSceneB == null)
        {
            GameObject go = new GameObject("DelayedActions");
            delayedActionsInSceneB = go.AddComponent<DelayedActionsInScene>();
        }
    }
    #endif
    #endregion

}

#region Essential Internal Classes

internal class DelayedActionsInScene : MonoBehaviour
{
    #region Fields
    private Dictionary<MonoBehaviour, List<DelayedActionInfo>> delayedActions = new();
    #if UNITY_EDITOR
    [SerializeField]
    private bool debugMode = true; // Toggle for debug mode

    public List<DelayedActionEntry> DebugList => debugList;
    [SerializeField]
    private List<DelayedActionEntry> debugList = new();
    #endif
    #endregion

    #region Public Internal Methods
    public void AddAction(DelayedActionInfo info, MonoBehaviour target)
    {
        if (delayedActions.ContainsKey(target))
        {
            delayedActions[target].Add(info);

        }
        else
        {
            delayedActions.Add(target, new List<DelayedActionInfo> { info });
        }
#if UNITY_EDITOR
        if (debugMode)
        {
            var entry = debugList.Find(e => e.target == target);
            if (entry == null)
            {
                entry = new DelayedActionEntry { target = target, actions = new List<DelayedActionInfo>() };
                debugList.Add(entry);
            }
            entry.actions.Add(info);
            Debug.Log($"Added action '{info.actionName}' with delay {info.delay} to target {target.name} ({target.GetType().Name})", target);
        }
#endif
    }
 
    public void Abort(MonoBehaviour target)
    {
        delayedActions.Remove(target);
#if UNITY_EDITOR
        if (debugMode)
            debugList.RemoveAll(e => e.target == target);
#endif
    }
    public void Abort(MonoBehaviour target, int ID)
    {
        delayedActions.TryGetValue(target, out List<DelayedActionInfo> list);
        list?.RemoveAll(info => info.id == ID);
#if UNITY_EDITOR
        if (debugMode)
        {
            var entry = debugList.Find(e => e.target == target);
            if (entry != null)
            {
                entry.actions.RemoveAll(info => info.id == ID);
                Debug.Log($"Aborted action with ID {ID} on target {target.name} ({target.GetType().Name})", target);
            }
        }
#endif


    }
    public void Abort(int ID)
    {
        foreach (var key in delayedActions.Keys)
        {
            Abort(key, ID);
        }
    }

    #endregion

    #region Internal Working
    private void Start()
    {
        DontDestroyOnLoad(gameObject); 
    }
   
    void Update()
    {
        var keys = new List<MonoBehaviour>(delayedActions.Keys);

        foreach (var id in keys)
        {
            if (id == null)
            {
                delayedActions.Remove(id);
                continue;
            }
            if (!delayedActions.ContainsKey(id))
            {
                continue;

            }

            List<DelayedActionInfo> list = delayedActions[id];


            for (int i = list.Count - 1; i >= 0; i--) //recorrer la lista y eliminar las cosas que ya hayan terminado
            {
                if (!delayedActions.ContainsKey(id))
                    break;
                var actionInfo = list[i];

                actionInfo.delay -= Time.deltaTime;

                if (actionInfo.delay <= 0)
                {

                    try
                    {
                        actionInfo.action?.Invoke();

                    }
                    catch (System.Exception e)
                    {
#if UNITY_EDITOR
                        Debug.LogError($"DelayedActions. Error executing action '{actionInfo.actionName}' (id: {actionInfo.id}) on target {id.name} ({id.GetType().Name}): {e.Message}. DelayedActions");
#endif
                    }

                    list.RemoveAt(i);
#if UNITY_EDITOR
                    if (debugMode)
                    {
                        var entry = debugList.Find(e => e.target == id);
                        if (entry != null)
                        {
                            entry.actions.Remove(actionInfo);
                            if (entry.actions.Count == 0)
                                debugList.Remove(entry);
                        }
                    }
#endif
                }



            }
            if (list.Count == 0) // Si la lista queda vacía, eliminar la entrada del diccionario
                delayedActions.Remove(id);

        }


    }
#if UNITY_EDITOR
    public void DebugActions()
    {
        foreach (var pair in delayedActions)
        {
            Debug.Log($"Target: {pair.Key.name} ({pair.Key.GetType().Name})", pair.Key);

            foreach (var actionInfo in pair.Value)
            {
                Debug.Log($" -> Action: {actionInfo.actionName}, Delay: {actionInfo.delay:F2}s , Id: {actionInfo.id}", pair.Key);
            }
        }
    }
#endif
    #endregion
}
[Serializable]
internal class DelayedActionInfo
{
   
    public Action action;
    
#if UNITY_EDITOR
    public string actionName;
#endif
    public float delay;
    public int id;
    static int idCounter = 1000000;
    public DelayedActionInfo()
    {
        id = idCounter++;
        idCounter = 1000000 + idCounter% 1000000; 
    }
    

}
#endregion

#region Editor Debugging

#if UNITY_EDITOR
[Serializable]
internal class DelayedActionEntry
{
    public MonoBehaviour target;

    public List<DelayedActionInfo> actions = new();
}
[CustomEditor(typeof(DelayedActionsInScene))]
internal class DelayedActionsInSceneEditor : Editor
{
    //private Dictionary<MonoBehaviour, bool> foldoutStates = new();

    public override void OnInspectorGUI()
    {
    

        DrawDefaultInspector();

        // Botón de refresco manual
        if (GUILayout.Button("🔁 Debug Actions"))
        {
            DelayedActionsInScene script = (DelayedActionsInScene)target;
            script.DebugActions(); // Este método ya lo tienes
        }
    }
}

[CustomPropertyDrawer(typeof(DelayedActionEntry))]
internal class DelayedActionEntryDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var targetProp = property.FindPropertyRelative("target");
        var actionsProp = property.FindPropertyRelative("actions");

        string targetName = targetProp.objectReferenceValue != null
            ? targetProp.objectReferenceValue.name
            : "Null Target";

        // Usa el nombre del objeto como label
        EditorGUI.PropertyField(position, property, new GUIContent(targetName), true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
[CustomPropertyDrawer(typeof(DelayedActionInfo))]
internal class DelayedActionInfoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var actionNameProp = property.FindPropertyRelative("actionName");
        var delayProp = property.FindPropertyRelative("delay");
        var idProp = property.FindPropertyRelative("id");

        float thirdWidth = position.width / 3;

        // Campo: actionName (a la izquierda)
        var actionRect = new Rect(position.x, position.y, thirdWidth - 5, position.height);
        EditorGUI.LabelField(actionRect, actionNameProp.stringValue);

        // Campo: delay (a la derecha)
        var delayRect = new Rect(position.x + thirdWidth + 5, position.y, thirdWidth - 5, position.height);
        EditorGUI.LabelField(delayRect, $"{delayProp.floatValue:F2}s");
        // Campo: id
        var idRect = new Rect(position.x + thirdWidth+thirdWidth + 5, position.y, thirdWidth+ thirdWidth - 5, position.height);
        EditorGUI.LabelField(idRect, $"{idProp.intValue}");
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
#endif
#endregion
