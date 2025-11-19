using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInversionCardAction : MonoBehaviour, ICardAction
{
    private enum InversionType { HEALONDAMAGE, REFLECTDAMAGE }
    [SerializeField] private InversionType inversionType;

    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
    private Transform playerTransform;

    [Header("Particles")]
    [SerializeField] private GameObject buffParticles;

    [Header("Audio")]
    [SerializeField] private ASoundPlayer buffSoundPlayer;
    [SerializeField] private int healOnDamageSoundIndex = 0;
    [SerializeField] private int reflectSoundIndex = 1;

    void Start()
    {
        if (buffParticles != null)
            buffParticles.SetActive(false);
    }

    public void ExecuteCardAction(CardObject cardObj)
    {
        switch (inversionType)
        {
            case InversionType.HEALONDAMAGE:
                playerTransform.GetComponent<PlayerCombat>().HealOnDamage = true;

                if (buffSoundPlayer != null)
                    buffSoundPlayer.PlaySound(healOnDamageSoundIndex);
                break;

            case InversionType.REFLECTDAMAGE:
                playerTransform.GetComponent<PlayerCombat>().ReflectDamage = true;

                if (buffSoundPlayer != null)
                    buffSoundPlayer.PlaySound(reflectSoundIndex);
                break;
        }

        GameObject buffP = Instantiate(buffParticles, playerTransform.position, Quaternion.identity);
        buffP.SetActive(true);
        buffP.transform.SetParent(playerTransform);

        cardObj.UsingCard = false;
    }
}