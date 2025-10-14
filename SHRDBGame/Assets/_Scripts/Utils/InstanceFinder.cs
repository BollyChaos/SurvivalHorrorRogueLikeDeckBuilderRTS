using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InstanceFinder : MonoBehaviour
{
#if UNITY_EDITOR
    public int targetID;

    [ContextMenu("Buscar objeto por ID")]
    void FindByID()
    {
        Object obj = EditorUtility.InstanceIDToObject(targetID);
        if (obj != null)
            Debug.Log($"Objeto encontrado: {obj.name}", obj);
        else
            Debug.LogWarning($"No se encontró ningún objeto con ID {targetID}");
    }
#endif
}
