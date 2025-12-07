using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
[RequireComponent(typeof(AudioSource))]
public class TioController : EnemyController
{
    [Header("IA Audio")]
    [SerializeField] private float hearingRange = 18f;

    [Header("Sonidos del Enemigo")]
    [SerializeField] private float roamingSoundInterval = 15f;

    private float roamingSoundTimer;
    private ASoundPlayer soundPlayer;
    private AudioSource audioSource;

    private Vector3? lastHeardSoundPosition;
    private bool hasDetectedPlayer = false;

    [Header("Ataque")]
    [SerializeField] GameObject slashPrefab;
    private float damage = 30f;
    private float health = 200f;

    private void Awake()
    {
        SetChaseSpeed(12);
        SetPatrolSpeed(3);
        base.Awake();
        SetState(new TioPatrolling(this));

        soundPlayer = GetComponent<ASoundPlayer>();
        audioSource = GetComponent<AudioSource>();

        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = hearingRange;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.playOnAwake = false;

        roamingSoundTimer = Random.Range(0f, roamingSoundInterval);
    }

    private void Update()
    {
        if (hasDetectedPlayer) return;

        roamingSoundTimer += Time.deltaTime;

        if (roamingSoundTimer >= roamingSoundInterval)
        {
            roamingSoundTimer = 0f;

            if (soundPlayer != null)
            {
                soundPlayer.PlaySound(0);
            }
        }
    }

    #region sonidos IA

    public void OnSoundHeard(Vector3 soundPosition)
    {
        float distance = Vector3.Distance(transform.position, soundPosition);
        if (distance <= hearingRange)
        {
            lastHeardSoundPosition = soundPosition;

            if (!hasDetectedPlayer)
            {
                hasDetectedPlayer = true;

                if (soundPlayer != null)
                {
                    soundPlayer.PlaySound(1);
                }
            }

            SetState(new TioChasing(this, soundPosition));
        }
    }

    public Vector3? GetLastHeardSoundPosition()
    {
        return lastHeardSoundPosition;
    }

    #endregion

    #region ataque

    public override void AttackPlayer()
    {
        /// Se Crea el slash para que el enemigo ataque
        GameObject sPrefab = Instantiate(slashPrefab, transform.position + transform.forward * 2, transform.rotation);
        sPrefab.SetActive(true);

        ParticleSystem ps = sPrefab.GetComponent<ParticleSystem>();
        ps.Play();

        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Player");
        }

        Destroy(sPrefab, 1f);
    }

    #endregion
}