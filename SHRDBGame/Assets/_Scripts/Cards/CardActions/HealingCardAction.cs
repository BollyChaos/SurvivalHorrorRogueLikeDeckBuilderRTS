using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingCardAction : ACardAction
{

    [SerializeField] private GameObject healParticles;
    [ReadOnly][SerializeField] GameObject hp;

    public override void ExecuteCardAction(CardObject cardObj)
    {
        if (hp == null)
            hp = Instantiate(healParticles, PlayerTransform);
        hp.SetActive(true);
        hp.GetComponent<ParticleSystem>().Play();


        GetComponent<ASoundPlayer>().PlaySound();

        //20%,30% y 40%???
        float healPercentage = (2 + (int)cardObj.card.cardRarity) * 0.1f;
        float healAmmount = PlayerTransform.GetComponent<PlayerCombat>().stats.MaxHealth * healPercentage;
        PlayerTransform.GetComponent<PlayerCombat>().Heal(healAmmount);
        cardObj.UsingCard = false;
        DelayedActions.Do(()=>hp.SetActive(false),duration,this);
    }


    public override void ResetCardAction()
    {

    }
}
