using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashCardAction : MonoBehaviour, ICardAction
{
    private enum TypeOfSlash { Axe, Knife }

    [SerializeField] GameObject slashPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private TypeOfSlash typeOfSlash;

    [Header("Audio")]
    [SerializeField] private AudioClip[] SwingSounds;      // Swing de hacha y cuchillo
    [SerializeField] private AudioClip[] ImpactSounds;     // Impactos solo hacha
    [SerializeField] private float swingVolume = 1f;

    private PlayerCombat playerCombat;
    private float damage = 0f;

    private AudioSource audioSource;

    Transform ICardAction.PlayerTransform { get => playerTransform; set => playerTransform = value; }

    private void Awake()
    {
        // Crear AudioSource local 2D si no existe
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
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

    void AxeAttack()
    {
        // Reproducir sonido de swing del hacha
        PlaySwingSound(SwingSounds);

        GameObject sPrefab = Instantiate(slashPrefab, playerTransform.position + playerTransform.forward * 2, playerTransform.rotation);
        sPrefab.SetActive(true);
        ParticleSystem ps = sPrefab.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Enemy");
            slash.SetImpactSounds(ImpactSounds); // Solo hacha tiene sonidos de impacto
        }

        Destroy(sPrefab, 5f);
    }

    void KnifeAttack()
    {
        // Reproducir sonido de swing del cuchillo
        PlaySwingSound(SwingSounds);

        GameObject sPrefab = Instantiate(slashPrefab, playerTransform.position + playerTransform.forward * 2, playerTransform.rotation);
        sPrefab.SetActive(true);
        ParticleSystem ps = sPrefab.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Enemy");
            // Para el cuchillo no ponemos sonidos de impacto
        }

        Destroy(sPrefab, 5f);
    }

    private void PlaySwingSound(AudioClip[] clips)
    {
        if (clips != null && clips.Length > 0 && audioSource != null)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            audioSource.PlayOneShot(clip, swingVolume);
        }
    }
}
