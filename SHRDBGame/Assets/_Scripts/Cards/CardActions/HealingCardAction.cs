using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingCardAction : MonoBehaviour, ICardAction
{
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject healParticles;

    [SerializeField] private AudioClip healSound;
    [SerializeField, Range(0f, 1f)] private float healSoundVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D
        }
    }

    public void ExecuteCardAction(CardObject cardObj)
    {
        GameObject hp = Instantiate(healParticles, playerTransform);
        hp.SetActive(true);
        hp.GetComponent<ParticleSystem>().Play();

        if (healSound != null && audioSource != null)
            audioSource.PlayOneShot(healSound, healSoundVolume);

        //20%,30% y 40%???
        float healPercentage = (2 + (int)cardObj.card.cardRarity) * 0.1f;
        float healAmmount = playerTransform.GetComponent<PlayerCombat>().stats.MaxHealth * healPercentage;
        playerTransform.GetComponent<PlayerCombat>().Heal(healAmmount);
        cardObj.UsingCard = false;
        Destroy(hp, 6f);
    }
}
