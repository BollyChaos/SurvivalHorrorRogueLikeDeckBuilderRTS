using System.Collections;
using System.Collections.Generic;
using Managers;
using Patterns.Singleton;
using UnityEngine;

[RequireComponent(typeof(GameObjectPool))]
public class ObjectPoolManager : ASingleton<ObjectPoolManager>, IManager
{
    public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;
    GameObjectPool gameObjectPool;

#region OBJECTPOOL
public GameObject Get(string goName,Transform parent=null)
    {
        return gameObjectPool.Get(goName,parent);
    }

#endregion
#region MANAGER
 public void StartManager()
    {
        gameObjectPool=GetComponent<GameObjectPool>();
    }
    public void LoadData()
    {
    }

    public void OnEnd()
    {
    }

    public void OnEndGame()
    {
    }

    public void OnStartGame()
    {
    }

    public void SaveData()
    {
    }

   #endregion
}
