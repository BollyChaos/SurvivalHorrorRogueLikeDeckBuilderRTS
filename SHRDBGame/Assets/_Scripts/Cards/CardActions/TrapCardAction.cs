using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class TrapCardAction : MonoBehaviour,ICardAction
{
    // Start is called before the first frame update
    [SerializeField] Transform playerTransform;
    [SerializeField] GameObject spikeTrapPrefab;

    public Transform PlayerTransform { get => playerTransform; set => playerTransform=value; }

    public void ExecuteCardAction(CardObject cardObj)
    {
        GameObject trap = Instantiate(spikeTrapPrefab, playerTransform.position+0.25f* UnityEngine.Vector3.down, UnityEngine.Quaternion.identity);
        trap.SetActive(true);
        cardObj.UsingCard = false;
    }
    
}
