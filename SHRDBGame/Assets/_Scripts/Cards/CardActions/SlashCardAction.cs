using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashCardAction : MonoBehaviour, ICardAction
{
    private enum TypeOfSlash { Axe, Knife }

    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private TypeOfSlash typeOfSlash;

    [Header("Audio")]
    [SerializeField] private AudioClip[] SwingSounds;

    [Header("Impact Sounds")]
    [SerializeField] private List<AudioClip> impactSoundsAxe;
    [SerializeField] private List<AudioClip> impactSoundsKnife;

    private PlayerCombat playerCombat;
    private float damage = 0f;
    private AudioSource audioSource;

    Transform ICardAction.PlayerTransform { get => playerTransform; set => playerTransform = value; }

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void ExecuteCardAction(CardObject cardObj)
    {
        playerCombat = playerTransform.GetComponent<PlayerCombat>();
        float baseDamage = playerCombat.stats.Attack;

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

    private void AxeAttack()
    {
        PlaySwingSound();

        GameObject sPrefab = Instantiate(slashPrefab, playerTransform.position + playerTransform.forward * 2, playerTransform.rotation);
        sPrefab.SetActive(true);

        // Asigna daño e impacto
        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Enemy");
            slash.SetImpactClips(impactSoundsAxe);
        }

        Destroy(sPrefab, 5f);
    }

    private void KnifeAttack()
    {
        GameObject sPrefab = Instantiate(slashPrefab, playerTransform.position + playerTransform.forward * 2, playerTransform.rotation);
        sPrefab.SetActive(true);

        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Enemy");
            slash.SetImpactClips(impactSoundsKnife);
        }

        Destroy(sPrefab, 5f);
    }

    private void PlaySwingSound()
    {
        if (SwingSounds != null && SwingSounds.Length > 0)
        {
            AudioClip clip = SwingSounds[Random.Range(0, SwingSounds.Length)];
            audioSource.PlayOneShot(clip);
        }
    }
}
