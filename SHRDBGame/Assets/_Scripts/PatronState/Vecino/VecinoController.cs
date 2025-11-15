using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VecinoController : EnemyController
{
    [SerializeField]private float hearingRange;
    private Vector3? lastHeardSoundPosition;
    [SerializeField] GameObject slashPrefab;
    private float damage = 20f;
    private float health = 100f;

    private void Awake()
    {
        SetChaseSpeed(7);
        SetPatrolSpeed(3);
        base.Awake();
        SetState(new VecinoPatrolling(this));
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
