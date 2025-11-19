using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TioController : EnemyController
{
    [SerializeField]private float hearingRange;
    private Vector3? lastHeardSoundPosition;
    [SerializeField] GameObject slashPrefab;
    private float damage = 30f;
    private float health = 200f;

    private void Awake()
    {
        SetChaseSpeed(12);
        SetPatrolSpeed(3);
        base.Awake();
        SetState(new TioPatrolling(this));
    }
    // private void OnEnable()
    // {
    //     SetState(new TioPatrolling(this));
    // }
    #region sonidos

    public void OnSoundHeard(Vector3 soundPosition)
    {
        float distance = Vector3.Distance(transform.position, soundPosition);
        if (distance <= hearingRange)
        {
            lastHeardSoundPosition = soundPosition;
            SetState(new TioChasing(this, soundPosition));
        }
    }
    public Vector3? GetLastHeardSoundPosition()
    {
        return lastHeardSoundPosition;
    }

    #endregion
    #region  ataque

    public override void AttackPlayer()
    {
        ///Se Crea el slash para que el enemigo ataque
        GameObject sPrefab = Instantiate(slashPrefab, transform.position + transform.forward * 2, transform.rotation);
        sPrefab.SetActive(true);
        ParticleSystem ps = sPrefab.GetComponent<ParticleSystem>();
        ps.Play();

        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Player");
        }
        Destroy(sPrefab, 5f);
    }
    #endregion
}
