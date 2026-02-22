using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCardAction : ACardAction
{
    [Header("Particles")]
    [SerializeField] GameObject freeCardPE;

    [Header("Audio")]
    [SerializeField] private ASoundPlayer freeCardSoundPlayer;
    [SerializeField] private int freeCardSoundIndex = 0;

    void Start()
    {
        if (freeCardPE != null)
            freeCardPE.SetActive(false);
    }

    public override void ExecuteCardAction(CardObject cardObj)
    {
        var ps = Instantiate(freeCardPE, PlayerTransform);
        ps.SetActive(true);
        //freeCardPE.transform.position = PlayerTransform.position;

        PlayerTransform.GetComponent<Economy>().NexPurchaseIsFree();

        if (freeCardSoundPlayer != null)
            freeCardSoundPlayer.PlaySound(freeCardSoundIndex);

        cardObj.UsingCard = false;
        Destroy(ps, 6);
    }

    public override void ResetCardAction()
    {

    }

}