using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class TioController : EnemyController
{
    [Header("IA Audio")]
    [SerializeField] private float hearingRange = 18f;

    [Header("ASoundPlayers separados")]
    [SerializeField] private ASoundPlayer detectionSoundPlayer;
    [SerializeField] private ASoundPlayer attackSoundPlayer;
    private Animator _animator;

    private Vector3? lastHeardSoundPosition;
    private bool hasDetectedPlayer = false;

    [Header("Ataque")]
    [SerializeField] GameObject slashPrefab;
    private float damage = 30f;
    private float health = 200f;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        SetChaseSpeed(12);
        SetPatrolSpeed(3);

        base.Awake();
        SetState(new TioPatrolling(this));
    }
    private void Update()
    {
        _animator.SetFloat("Speed", GetAgent().velocity.magnitude);
        base.Update();

    }

    #region sonidos IA

    public void OnSoundHeard(Vector3 soundPosition)
    {
        if (!gameObject.activeInHierarchy || health <= 0) return;

        float distance = Vector3.Distance(transform.position, soundPosition);

        if (distance <= hearingRange)
        {
            lastHeardSoundPosition = soundPosition;

            if (!hasDetectedPlayer)
            {
                hasDetectedPlayer = true;

                // if (detectionSoundPlayer != null)
                // {
                //     detectionSoundPlayer.PlaySound(0);
                // }
            }

            SetState(new TioChasing(this, soundPosition));
        }
    }

    public void StopDetectionSound()
    {
        if (detectionSoundPlayer != null && hasDetectedPlayer)
        {
            detectionSoundPlayer.StopLoop();
            hasDetectedPlayer = false;
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
        if (attackSoundPlayer != null)
        {
            attackSoundPlayer.PlaySound(0);
        }

        GameObject sPrefab = Instantiate(
            slashPrefab,
            transform.position + transform.forward * 2,
            transform.rotation
        );

        sPrefab.SetActive(true);

        ParticleSystem ps = sPrefab.GetComponent<ParticleSystem>();
        ps.Play();

        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Player");
        }

        Destroy(sPrefab, 1f);
        _animator.SetTrigger("Attacking");
    }

    #endregion

    public void OnReset()
    {
        base.OnReset();
        SetState(new TioPatrolling(this));
    }
}