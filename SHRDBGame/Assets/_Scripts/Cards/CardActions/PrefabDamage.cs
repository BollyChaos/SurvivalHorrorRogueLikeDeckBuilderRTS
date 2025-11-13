using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabDamage : MonoBehaviour
{
    private float damage = 0f;
    [SerializeField]
    private string targetTag = "Enemy";
    public string Tag { get => targetTag; }

    // --- NUEVO: sonidos de impacto ---
    [SerializeField] private AudioClip[] impactSounds;
    [SerializeField] private float impactVolume = 1f;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        //lo voy a destruir como apaño en slashcardaction
        // Destroy(gameObject, lifetime); por que???? ponlo en una funcion y controlas cuando se destruye no solo nada mas crearlo
    }

    public void Initialize(float dmg, string targetTag = "Enemy")
    {
        damage = dmg;
        this.targetTag = targetTag;
    }

    // NUEVO: método para asignar los sonidos desde SlashCardAction
    public void SetImpactSounds(AudioClip[] clips)
    {
        impactSounds = clips;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            EnemyCombat enemy = other.GetComponent<EnemyCombat>();
            if (enemy != null)
            {
                enemy.stats.TakeDamage(damage);
                Debug.Log($"{other.name} took {damage} damage from attack!");

                // NUEVO: reproducir sonido de impacto aleatorio
                if (impactSounds != null && impactSounds.Length > 0)
                {
                    AudioClip clip = impactSounds[Random.Range(0, impactSounds.Length)];
                    audioSource.PlayOneShot(clip, impactVolume);
                }
            }
        }
    }
}
