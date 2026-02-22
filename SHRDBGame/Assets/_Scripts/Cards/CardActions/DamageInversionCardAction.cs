using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInversionCardAction : ACardAction
{
    private enum InversionType { HEALONDAMAGE, REFLECTDAMAGE }
    [SerializeField] private InversionType inversionType;

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

    public override void ExecuteCardAction(CardObject cardObj)
    {
        switch (inversionType)
        {
            case InversionType.HEALONDAMAGE:
                PlayerTransform.GetComponent<PlayerCombat>().HealOnDamage = true;

                if (buffSoundPlayer != null)
                    buffSoundPlayer.PlaySound(healOnDamageSoundIndex);
                break;

            case InversionType.REFLECTDAMAGE:
                PlayerTransform.GetComponent<PlayerCombat>().ReflectDamage = true;

                if (buffSoundPlayer != null)
                    buffSoundPlayer.PlaySound(reflectSoundIndex);
                break;
        }

        GameObject buffP = Instantiate(buffParticles, PlayerTransform.position, Quaternion.identity);
        buffP.SetActive(true);
        buffP.transform.SetParent(PlayerTransform);

        cardObj.UsingCard = false;
        Destroy(buffP,6);
        //Release();
    }

    public override void ResetCardAction()
    {
    }


}