using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class BuffCardAction : MonoBehaviour, ICardAction
{
    private enum BuffType { Speed, Damage, Invencibility }

    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
    [SerializeField] BuffType buffType;
    [SerializeField] private Transform playerTransform;

    [SerializeField] private GameObject particles;
    [SerializeField] private float buffTime = 5f;

    private bool HasUsedBuff = false;
    private float buffTimeCounter = 0f;

    private ASoundPlayer soundPlayer;

    void Start()
    {
        soundPlayer = GetComponent<ASoundPlayer>();
    }

    public void ExecuteCardAction(CardObject cardObj)
    {
        buffTimeCounter += buffTime;
        HasUsedBuff = true;

        GameObject bp = Instantiate(particles, playerTransform);
        bp.SetActive(true);
        bp.GetComponent<ParticleSystem>().Play();

        switch (buffType)
        {
            case BuffType.Speed:
                float speedPercentage = 1 + (2 + (int)cardObj.card.cardRarity) * 0.1f;
                playerTransform.GetComponent<PlayerCombat>().stats.ChangeSpeedMultiplier(speedPercentage);

                FootstepPlayer fp = playerTransform.GetComponent<FootstepPlayer>();
                if (fp != null) fp.boostActive = true;

                if (soundPlayer != null)
                    soundPlayer.PlayRandomSound();
                break;

            case BuffType.Damage:
                float damagePercentage = 1 + (2 + (int)cardObj.card.cardRarity) * 0.1f;
                playerTransform.GetComponent<PlayerCombat>().stats.AttackMultiplier = damagePercentage;

                if (soundPlayer != null)
                    soundPlayer.PlayRandomSound();
                break;

            case BuffType.Invencibility:
                playerTransform.GetComponent<PlayerCombat>().stats.Invencibility = true;

                if (soundPlayer != null)
                    soundPlayer.PlayRandomSound();
                break;
        }

        cardObj.UsingCard = false;
        Destroy(bp, 6f);
    }

    private void Update()
    {
        if (!HasUsedBuff) return;

        buffTimeCounter -= Time.deltaTime;

        if (buffTimeCounter <= 0f)
        {
            HasUsedBuff = false;

            switch (buffType)
            {
                case BuffType.Speed:
                    playerTransform.GetComponent<PlayerCombat>().stats.ChangeSpeedMultiplier(1f);
                    FootstepPlayer fp = playerTransform.GetComponent<FootstepPlayer>();
                    if (fp != null) fp.boostActive = false;
                    break;

                case BuffType.Damage:
                    playerTransform.GetComponent<PlayerCombat>().stats.AttackMultiplier = 1f;
                    break;

                case BuffType.Invencibility:
                    playerTransform.GetComponent<PlayerCombat>().stats.Invencibility = false;
                    break;
            }
        }
    }
}