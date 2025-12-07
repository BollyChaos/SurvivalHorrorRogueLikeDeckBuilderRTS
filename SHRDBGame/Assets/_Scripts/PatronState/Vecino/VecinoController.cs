using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(ASoundPlayer))]
[RequireComponent(typeof(AudioSource))]
public class VecinoController : EnemyController
{
    [Header("IA Audio")]
    [SerializeField] private float hearingRange = 15f;

    [Header("Sonidos del Enemigo")]
    [SerializeField] private float soundInterval = 15f;

    private float soundTimer;
    private ASoundPlayer soundPlayer;
    private AudioSource audioSource;

    private Vector3? lastHeardSoundPosition;

    [Header("Ataque")]
    [SerializeField] GameObject slashPrefab;
    private float damage = 20f;
    private float health = 100f;

    private IObjectPool<VecinoController> enemyPool;

    public void SetPool(IObjectPool<VecinoController> pool)
    {
        enemyPool = pool;
    }

    private void Awake()
    {
        SetChaseSpeed(7);
        SetPatrolSpeed(3);

        base.Awake();
        SetState(new VecinoPatrolling(this));

        soundPlayer = GetComponent<ASoundPlayer>();
        audioSource = GetComponent<AudioSource>();

        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 2f;
        audioSource.maxDistance = hearingRange;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        soundTimer = Random.Range(0f, soundInterval);
    }

    private void Update()
    {
        soundTimer += Time.deltaTime;

        if (soundTimer >= soundInterval)
        {
            soundTimer = 0f;

            if (soundPlayer != null)
            {
                soundPlayer.PlayRandomSound();
            }
        }
    }

    #region sonidos

    public void OnSoundHeard(Vector3 soundPosition)
    {
        float distance = Vector3.Distance(transform.position, soundPosition);
        if (distance <= hearingRange)
        {
            lastHeardSoundPosition = soundPosition;
            SetState(new VecinoChasing(this, soundPosition));
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