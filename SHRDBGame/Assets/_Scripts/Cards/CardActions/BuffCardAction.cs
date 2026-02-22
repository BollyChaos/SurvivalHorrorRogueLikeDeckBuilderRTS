using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class BuffCardAction : ACardAction
{
    private enum BuffType { Speed, Damage, Invencibility }

    [SerializeField] BuffType buffType;

    [SerializeField] private GameObject particles;
    [SerializeField] private float buffTime = 5f;

    private bool HasUsedBuff = false;
    private float buffTimeCounter = 0f;

    private ASoundPlayer soundPlayer;

    void Start()
    {
        soundPlayer = GetComponent<ASoundPlayer>();
    }

    public override void ExecuteCardAction(CardObject cardObj)
    {
        buffTimeCounter += buffTime;
        HasUsedBuff = true;

        GameObject bp = Instantiate(particles, PlayerTransform);
        
        bp.SetActive(true);
        bp.GetComponent<ParticleSystem>().Play();

        switch (buffType)
        {
            case BuffType.Speed:
                float speedPercentage = 1 + (2 + (int)cardObj.card.cardRarity) * 0.1f;
                PlayerTransform.GetComponent<PlayerCombat>().stats.ChangeSpeedMultiplier(speedPercentage);

                FootstepPlayer fp = PlayerTransform.GetComponent<FootstepPlayer>();
                if (fp != null) fp.boostActive = true;

                if (soundPlayer != null)
                    soundPlayer.PlayRandomSound();
                break;

            case BuffType.Damage:
                float damagePercentage = 1 + (2 + (int)cardObj.card.cardRarity) * 0.1f;
                PlayerTransform.GetComponent<PlayerCombat>().stats.AttackMultiplier = damagePercentage;

                if (soundPlayer != null)
                    soundPlayer.PlayRandomSound();
                break;

            case BuffType.Invencibility:
                PlayerTransform.GetComponent<PlayerCombat>().stats.Invencibility = true;

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
                    PlayerTransform.GetComponent<PlayerCombat>().stats.ChangeSpeedMultiplier(1f);
                    FootstepPlayer fp = PlayerTransform.GetComponent<FootstepPlayer>();
                    if (fp != null) fp.boostActive = false;
                    break;

                case BuffType.Damage:
                    PlayerTransform.GetComponent<PlayerCombat>().stats.AttackMultiplier = 1f;
                    break;

                case BuffType.Invencibility:
                    PlayerTransform.GetComponent<PlayerCombat>().stats.Invencibility = false;
                    break;
            }
        }
    }

    public override void ResetCardAction()
    {
        
    }
}