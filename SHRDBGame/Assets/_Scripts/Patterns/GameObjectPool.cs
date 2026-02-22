using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    [SerializeField]
    Transform parentTransform;
    [SerializeField]
    private List<ObjectPoolEntry> pools = new();
    readonly Dictionary<string, ObjectPoolEntry> lookup = new();
    // Start is called before the first frame update
    void Awake()
    {
        // construir lookup
        foreach (var entry in pools)
        {
            if (entry.prefab.name == null) continue;

            lookup[entry.prefab.name] = entry;

            // prewarm 
            for (int i = 0; i < entry.prewarmCount; i++)
            {
                var go = InstantiateFromPrefab(entry.prefab);
                go.SetActive(false);
                entry.pool.Enqueue(go);

            }
        }
    }
    public GameObject InstantiateFromPrefab(GameObject prefab,Transform parent=null)
    {
        var go = Instantiate(prefab);
        var poolable=go.GetComponent<IPoolableObject>();
        if(poolable==null)Debug.LogError("Object is not poolable, it does not use IPoolable interface");
        poolable.GameObjectPool = this;

        go.name=prefab.name;
        if (parent != null)
        {
            go.transform.SetParent(parent);
            
        }
        else if(parentTransform != null)
        {
            go.transform.SetParent(parentTransform);
            
        }
        else
        {
            go.transform.SetParent(transform);
        }
        return go;
    }

    
    public GameObject Get(string prefabName,Transform parent=null)
    {
        if (!lookup.TryGetValue(prefabName, out var entry))
        {
            Debug.LogError($"No pool for prefab {prefabName}");
            return null;
        }

        if (entry.pool.Count > 0)
        {
            var go = entry.pool.Dequeue();
            go.SetActive(true);
            
            //a su interfaz de ipooleable pasar la referencia de la pool
            return go;
        }

        return InstantiateFromPrefab(entry.prefab,parent);
    }
    public void Release(GameObject instance)
    {
        if (!lookup.TryGetValue(instance.name, out var entry))
        {
            Destroy(instance);
            Debug.Log("Not found, destroyed");
            return;
        }

        instance.SetActive(false);
        entry.pool.Enqueue(instance);
    }
}
[System.Serializable]
public class ObjectPoolEntry
{
    public GameObject prefab;
    public int prewarmCount = 0;

    [System.NonSerialized]
    public Queue<GameObject> pool = new();
}
