using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCardAction : MonoBehaviour,ICardAction
{
    // Start is called before the first frame update
    [SerializeField] GameObject freeCardPE;

    private Transform playerTransform;
    public Transform PlayerTransform { get => playerTransform; set =>playerTransform=value; }
    void Start()
    {
        freeCardPE.SetActive(false);
        
    }
    public void ExecuteCardAction(CardObject cardObj)
    {
        freeCardPE.SetActive(false);//como la animacion esta en playonawake esto le mete un reseteo, no estoy loco, aun
        freeCardPE.SetActive(true);
        freeCardPE.transform.position = playerTransform.position;
        playerTransform.GetComponent<Economy>().NexPurchaseIsFree();
        cardObj.UsingCard = false;
    }

   
}
