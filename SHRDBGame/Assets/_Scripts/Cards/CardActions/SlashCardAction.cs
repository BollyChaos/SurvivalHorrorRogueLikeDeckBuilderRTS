using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashCardAction : MonoBehaviour, ICardAction
{
    private enum TypeOfSlash { Axe, Knife }
    [Tooltip("Multiplicador de arma(el jugador empieza con 10 daño base x 1 de multiplicador propio)")]
    [SerializeField] float toolMultiplier = 2f;
    [SerializeField] private GameObject slashPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private TypeOfSlash typeOfSlash;

    // [Header("Audio")] Aqui no hay audio, es la clase controladora, manda a que la clase de audio ponga los sonidos pero aqui no se conocen
    // [SerializeField] private AudioClip[] SwingSounds;

    // [Header("Impact Sounds")]
    // [SerializeField] private List<AudioClip> impactSoundsAxe;
    // [SerializeField] private List<AudioClip> impactSoundsKnife;

    private PlayerCombat playerCombat;
    private float damage = 0f;
    // private AudioSource audioSource;

    Transform ICardAction.PlayerTransform { get => playerTransform; set => playerTransform = value; }

    // private void Awake()
    // {
    //     audioSource = gameObject.AddComponent<AudioSource>();
    //     audioSource.playOnAwake = false;
    // }

    public void ExecuteCardAction(CardObject cardObj)
    {
        playerCombat = playerTransform.GetComponent<PlayerCombat>();
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

        GameObject sPrefab = Instantiate(slashPrefab, playerTransform.position + playerTransform.forward * 2 + Vector3.up, playerTransform.rotation);
        sPrefab.SetActive(true);

        // Asigna da�o e impacto
        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Enemy");
            // slash.SetImpactClips(impactSoundsAxe);
        }

        Destroy(sPrefab, 5f);
    }

    private void KnifeAttack()
    {
        GetComponent<ASoundPlayer>().PlayRandomSound();

        GameObject sPrefab = Instantiate(slashPrefab, playerTransform.position + playerTransform.forward * 1.5f + Vector3.up, playerTransform.rotation);
        sPrefab.SetActive(true);

        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Enemy");
            // slash.SetImpactClips(impactSoundsKnife);
        }

        Destroy(sPrefab, 5f);
    }

    // private void PlaySwingSound()
    // {
    //     if (SwingSounds != null && SwingSounds.Length > 0)
    //     {
    //         AudioClip clip = SwingSounds[Random.Range(0, SwingSounds.Length)];
    //         audioSource.PlayOneShot(clip);
    //     }
    // }
}
