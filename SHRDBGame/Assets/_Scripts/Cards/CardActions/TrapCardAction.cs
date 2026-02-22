using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class TrapCardAction : ACardAction
{
    [SerializeField] GameObject spikeTrapPrefab;

    private ASoundPlayer soundPlayer;

    void Start()
    {
        soundPlayer = GetComponent<ASoundPlayer>();
    }

    public override void ExecuteCardAction(CardObject cardObj)
    {
        GameObject trap = Instantiate(
            spikeTrapPrefab,
            PlayerTransform.position + 0.25f * Vector3.down,
            Quaternion.identity
        );

        trap.SetActive(true);

        if (soundPlayer != null)
            soundPlayer.PlayRandomSound();

        cardObj.UsingCard = false;
    }

    public override void ResetCardAction()
    {
       
    }

}