using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCardAction : MonoBehaviour, ICardAction
{
    [Header("Particles")]
    [SerializeField] GameObject freeCardPE;

    private Transform playerTransform;
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }

    [Header("Audio")]
    [SerializeField] private ASoundPlayer freeCardSoundPlayer;
    [SerializeField] private int freeCardSoundIndex = 0;

    void Start()
    {
        if (freeCardPE != null)
            freeCardPE.SetActive(false);
    }

    public void ExecuteCardAction(CardObject cardObj)
    {
        freeCardPE.SetActive(false);
        freeCardPE.SetActive(true);

        freeCardPE.transform.position = playerTransform.position;

        playerTransform.GetComponent<Economy>().NexPurchaseIsFree();

        if (freeCardSoundPlayer != null)
            freeCardSoundPlayer.PlaySound(freeCardSoundIndex);

        cardObj.UsingCard = false;
    }
}