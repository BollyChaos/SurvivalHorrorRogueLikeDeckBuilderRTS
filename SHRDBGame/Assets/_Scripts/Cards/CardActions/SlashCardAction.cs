using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashCardAction : MonoBehaviour, ICardAction
{
    private enum TypeOfSlash { Axe, Knife }
    [SerializeField] GameObject slashPrefab;

    [SerializeField] public Transform playerTransform;
    [SerializeField] private TypeOfSlash typeOfSlash;

    private PlayerCombat playerCombat;
    float damage = 0f;

    Transform ICardAction.PlayerTransform { get => playerTransform; set => playerTransform = value; }
    //[Header("Slash Settings")]

    //[SerializeField] float spreadAngle = 60f;
    //[SerializeField] int bulletsCount = 4;

    private void Awake()
    {
        playerCombat = playerTransform.GetComponent<PlayerCombat>();
    }

    public void ExecuteCardAction(CardObject cardObj)
    {
        float baseDamage = playerCombat.stats.attack;
        

        switch (typeOfSlash)
        {
            case TypeOfSlash.Axe:
                damage = baseDamage * 3f;
                AxeAttack();

                break;
            case TypeOfSlash.Knife:
                damage = baseDamage * 2f;
                KnifeAttack();

                break;
        }
        cardObj.UsingCard = false;
    }
    void AxeAttack()
    {
        GameObject sPrefab = Instantiate(slashPrefab, playerTransform.position + playerTransform.forward * 2, playerTransform.rotation);
        ParticleSystem ps = sPrefab.GetComponent<ParticleSystem>();
        ps.Play();

        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Enemy");
        }
    }
    void KnifeAttack()
    {
        GameObject sPrefab = Instantiate(slashPrefab, playerTransform.position + playerTransform.forward * 2, playerTransform.rotation);
        ParticleSystem ps = sPrefab.GetComponent<ParticleSystem>();
        ps.Play();

        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Enemy");
        }
    }
}
