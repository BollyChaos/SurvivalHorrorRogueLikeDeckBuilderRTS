using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingCardAction : ACardAction
{

    [SerializeField] private GameObject healParticles;


    public override void ExecuteCardAction(CardObject cardObj)
    {
        GameObject hp = Instantiate(healParticles, PlayerTransform);
        hp.SetActive(true);
        hp.GetComponent<ParticleSystem>().Play();

        // if (healSound != null && audioSource != null)
        //     audioSource.PlayOneShot(healSound, healSoundVolume);
        GetComponent<ASoundPlayer>().PlaySound();

        //20%,30% y 40%???
        float healPercentage = (2 + (int)cardObj.card.cardRarity) * 0.1f;
        float healAmmount = PlayerTransform.GetComponent<PlayerCombat>().stats.MaxHealth * healPercentage;
        PlayerTransform.GetComponent<PlayerCombat>().Heal(healAmmount);
        cardObj.UsingCard = false;
       //DelayedActions.Do(Release,6f,this);
    }

    public override void ResetCardAction()
    {
      
    }
}
