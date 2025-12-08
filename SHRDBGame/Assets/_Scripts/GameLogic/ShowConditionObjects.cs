using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class ShowConditionObjects : MonoBehaviour
{
    // Start is called before the first frame update
[SerializeField]    List<GameObject> conditionObjects = new List<GameObject>();
void Awake()
    {
        foreach(var obj in conditionObjects)
        {
            obj.SetActive(false);
        }
    }
    void Start()
    {
         if(GameManager.Instance.GetValue<bool>("GivenTeddyBear"))
        {
           conditionObjects[0].SetActive(true);
        }
        if(GameManager.Instance.GetValue<float>("NCardsUsed")>=20)
        {
              conditionObjects[1].SetActive(true);
        }
        if(GameManager.Instance.GetValue<float>("NTimesWon")>=1)
        {
          conditionObjects[2].SetActive(true);
        }
       
    }


    
}
