using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableObject : MonoBehaviour,IPoolableObject
{
    public GameObjectPool GameObjectPool { get => gameObjectPool; set => gameObjectPool=value; }
    private GameObjectPool gameObjectPool;
    public void Release()
    {
       if(gameObjectPool)
            gameObjectPool.Release(this.gameObject);
    }

   
}
