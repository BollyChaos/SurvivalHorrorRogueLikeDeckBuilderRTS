using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HijoController : EnemyController
{
    //atributos específicos del Hijo
    private float _currentHealth;
    private float _maxHealth;
    private bool canPhaseTwo = true;
    private bool canPhaseThree = true;
    
    [SerializeField]private float damage = 50f;
    [SerializeField]private float rangeDamage = 20f;
    [SerializeField]private int speed = 5;
    private int attacksRecieved=0;
    [SerializeField] GameObject slashPrefab;
    [SerializeField] GameObject BulletPrefab;
    private void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        _currentHealth = GetComponent<EnemyCombat>().stats.CurrentHealth;
        _maxHealth = GetComponent<EnemyCombat>().stats.MaxHealth;
        //SetState(new HijoPatrolling(this));
    }
    void Update()
    {
        base.Update();
        _currentHealth = GetComponent<EnemyCombat>().stats.CurrentHealth;
        if (_currentHealth <= _maxHealth * 0.6f && _currentHealth > _maxHealth * 0.2f)
        {
            if (canPhaseTwo)
            {
                // Cambiar a fase 2
                PhaseTwo();

                canPhaseTwo = false;
            }
        }
        if (_currentHealth <= _maxHealth * 0.2f)
        {
            if (canPhaseThree)
            {
                // Cambiar a fase 3
                PhaseThree();
                canPhaseThree = false;
            }
            
        }
    }
    public override void PhaseTwo()
    {
        //SetState(new HijoPhaseTwo(this));
    }
    public override void PhaseThree()
    {
        //SetState(new HijoPhaseThree(this));
    }

    public override float GetCurrentHealth()
    {
        return _currentHealth;
    }
    public override float GetMaxHealth()
    {
        return _maxHealth;
    }

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
        Destroy(sPrefab, 1f);
    }
    public override void RangeAttackPlayer()
    {
        GameObject sPrefab = Instantiate(BulletPrefab, transform.position + transform.forward * 2, transform.rotation);
        sPrefab.SetActive(true);
        sPrefab.GetComponent<MoveInDirection>().direction = transform.forward;
        ParticleSystem ps = sPrefab.GetComponent<ParticleSystem>();
        ps.Play();

        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(rangeDamage, "Player");
        }
        Destroy(sPrefab, 5f);
    }

    public override int GetChaseSpeed()
    {
        return speed;
    }

    public override void RecordAttack()
    {
        attacksRecieved++;
    }
    public override void ClearAttackRecords()
    {
        attacksRecieved = 0;
    }
    public override void PopAttack()
    {
        if (attacksRecieved > 0)
        {
            attacksRecieved--;
        }
    }
    public override int NAttacksRecieved()
    {
        return attacksRecieved;
    }

}
