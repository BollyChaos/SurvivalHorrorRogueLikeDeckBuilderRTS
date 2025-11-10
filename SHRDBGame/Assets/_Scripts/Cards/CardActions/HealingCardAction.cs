using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingCardAction : MonoBehaviour, ICardAction
{
    public Transform PlayerTransform { get => playerTransform; set => playerTransform=value; }
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject healParticles;
    public void ExecuteCardAction(CardObject cardObj)
    {
        GameObject hp = Instantiate(healParticles, playerTransform);
        hp.GetComponent<ParticleSystem>().Play();
        //20%,30% y 40%???
        float healPercentage = (2 + (int)cardObj.card.cardRarity) * 0.1f;
        float healAmmount = playerTransform.GetComponent<PlayerCombat>().stats.MaxHealth * healPercentage;
        playerTransform.GetComponent<PlayerCombat>().stats.Heal(healAmmount);
        cardObj.UsingCard = false;
        Destroy(hp, 6f);
    }
   


}
