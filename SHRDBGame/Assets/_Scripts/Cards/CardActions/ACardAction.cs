using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ACardAction : MonoBehaviour, ICardAction//, IPooleableObject todavia no???
{
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
    Transform playerTransform;
    // public GameObjectPool GameObjectPool { get => gameObjectPool; set => gameObjectPool = value; }
    // GameObjectPool gameObjectPool;
    public abstract void ExecuteCardAction(CardObject cardObj);

    [SerializeField]protected float duration=0.5f;

    // public virtual void Release()
    // {
    //     ResetCardAction();
    //     if (gameObjectPool != null)
    //         gameObjectPool.Release(gameObject);
    // }

    public abstract void ResetCardAction();
}
