using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInversionCardAction : MonoBehaviour,ICardAction
{
    private enum InversionType { HEALONDAMAGE, REFLECTDAMAGE }
    [SerializeField] private InversionType inversionType;
    

    public Transform PlayerTransform { get => playerTransform; set =>playerTransform=value; }
    private Transform playerTransform;

    [SerializeField]
    GameObject buffParticles;
    void Start()
    {
        buffParticles.SetActive(false);
    } 
    public void ExecuteCardAction(CardObject cardObj)
    {
        switch (inversionType)
        {
            case InversionType.HEALONDAMAGE:
        playerTransform.GetComponent<PlayerCombat>().HealOnDamage = true;

                break;
            case InversionType.REFLECTDAMAGE:
        playerTransform.GetComponent<PlayerCombat>().ReflectDamage = true;

                break;
        }
        GameObject buffP = Instantiate(buffParticles, playerTransform.position, Quaternion.identity);
        buffP.SetActive(true);
        buffP.transform.SetParent(playerTransform);

        cardObj.UsingCard = false;
    }
}
