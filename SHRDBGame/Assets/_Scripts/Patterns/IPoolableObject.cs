using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolableObject 
{
    GameObjectPool GameObjectPool{get;set;}
    public void Release();//volver a la pool
}
