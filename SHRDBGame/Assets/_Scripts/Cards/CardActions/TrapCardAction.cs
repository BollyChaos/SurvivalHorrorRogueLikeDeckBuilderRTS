using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class TrapCardAction : MonoBehaviour, ICardAction
{
    [SerializeField] Transform playerTransform;
    [SerializeField] GameObject spikeTrapPrefab;

    private ASoundPlayer soundPlayer;

    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }

    void Start()
    {
        soundPlayer = GetComponent<ASoundPlayer>();
    }

    public void ExecuteCardAction(CardObject cardObj)
    {
        GameObject trap = Instantiate(
            spikeTrapPrefab,
            playerTransform.position + 0.25f * Vector3.down,
            Quaternion.identity
        );

        trap.SetActive(true);

        if (soundPlayer != null)
            soundPlayer.PlayRandomSound();

        cardObj.UsingCard = false;
    }
}