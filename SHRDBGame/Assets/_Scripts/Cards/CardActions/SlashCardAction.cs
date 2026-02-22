using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashCardAction : ACardAction
{
    private enum TypeOfSlash { Axe, Knife }
    [Tooltip("Multiplicador de arma(el jugador empieza con 10 daño base x 1 de multiplicador propio)")]
    [SerializeField] float toolMultiplier = 2f;
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private TypeOfSlash typeOfSlash;

    private PlayerCombat playerCombat;
    private float damage = 0f;


    #region CARDLOGIC
    public override void ExecuteCardAction(CardObject cardObj)
    {
        playerCombat = PlayerTransform.GetComponent<PlayerCombat>();
        float baseDamage = playerCombat.stats.Attack;
        damage = baseDamage * toolMultiplier;


        switch (typeOfSlash)
        {
            case TypeOfSlash.Axe:
                AxeAttack();
                break;
            case TypeOfSlash.Knife:
                KnifeAttack();
                break;
        }

        cardObj.UsingCard = false;
    }

    private void AxeAttack()
    {
        GetComponent<ASoundPlayer>().PlayRandomSound();

        //esto es feo de cojones, cambiar
        GameObject slashPrefab = ObjectPoolManager.Instance.Get("AxeCardPrefab", PlayerTransform);
        //Instantiate(slashPrefab, PlayerTransform.position + PlayerTransform.forward * 2 + Vector3.up, PlayerTransform.rotation);
        slashPrefab.SetActive(true);
        slashPrefab.transform.position = PlayerTransform.position + PlayerTransform.forward * 2 + Vector3.up;
        slashPrefab.transform.rotation = PlayerTransform.rotation;

        // Asigna da�o e impacto
        PrefabDamage slash = slashPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Enemy");
            // slash.SetImpactClips(impactSoundsAxe);
        }

        //DelayedActions.Do(Release,0.5f,this);
        DelayedActions.Do(slashPrefab.GetComponent<PoolableObject>().Release, duration, this);

    }

    private void KnifeAttack()
    {
        GetComponent<ASoundPlayer>().PlayRandomSound();

        GameObject slashPrefab = ObjectPoolManager.Instance.Get("KnifeCardPrefab", PlayerTransform);
        //Instantiate(slashPrefab, PlayerTransform.position + PlayerTransform.forward * 1.5f + Vector3.up, PlayerTransform.rotation);
        slashPrefab.SetActive(true);
        slashPrefab.transform.position = PlayerTransform.position + PlayerTransform.forward * 2 + Vector3.up;
        slashPrefab.transform.rotation = PlayerTransform.rotation;

        PrefabDamage slash = slashPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Enemy");
            // slash.SetImpactClips(impactSoundsKnife);
        }

        DelayedActions.Do(slashPrefab.GetComponent<PoolableObject>().Release, duration, this);

    }

    public override void ResetCardAction()
    {
        damage = 0;
    }
    #endregion

}
