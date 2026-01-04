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

    [SerializeField] private float damage = 50f;
    [SerializeField] private float rangeDamage = 20f;
    [SerializeField] private int speed = 5;
    private int attacksRecieved = 0;
    private bool canReciveAttacks = false;
    [SerializeField] GameObject slashPrefab;
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] GameObject flashbangLightPrefab;
    [SerializeField] GameObject firePrefab;
    [SerializeField] RoomTrigger roomTrigger;
    private BoxCollider boxCollider;
    private GameObject _player;
    private bool canRangeAttack = true;
    [SerializeField] private float rangeAttackCooldown = 1.5f;
    private void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        _currentHealth = GetComponent<EnemyCombat>().stats.CurrentHealth;
        _maxHealth = GetComponent<EnemyCombat>().stats.MaxHealth;
        flashbangLightPrefab.SetActive(false);
        boxCollider = roomTrigger.GetComponent<BoxCollider>();
        _player = GameObject.FindGameObjectWithTag("Player");
        SetState(new HijoIdle(this));
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
                StartCoroutine(PhaseTwo());

                canPhaseTwo = false;
            }
        }
        if (_currentHealth <= _maxHealth * 0.2f)
        {
            if (canPhaseThree)
            {
                // Cambiar a fase 3
                StartCoroutine(PhaseThree());
                canPhaseThree = false;
            }

        }
    }
    private IEnumerator PhaseTwo()
    {
        Debug.Log("Entering Phase Two");
        GetAgent().isStopped = true;
        yield return new WaitForSeconds(1.5f);
        GetAgent().isStopped = false;
        SetCanReciveAttacks(true);
        SetState(new HijoChasing2(this));
    }
    private IEnumerator PhaseThree()
    {
        Debug.Log("Entering Phase Three");
        GetAgent().isStopped = true;
        yield return new WaitForSeconds(1.5f);
        GetAgent().isStopped = false;
        SetCanReciveAttacks(true);
        //SetState(new HijoChasing2(this));
    }

    public override float GetCurrentHealth()
    {
        return _currentHealth;
    }
    public override float GetMaxHealth()
    {
        return _maxHealth;
    }

    public override GameObject GetPlayer()
    {
        return _player;
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

        PrefabDamage bullet = sPrefab.GetComponent<PrefabDamage>();
        if (bullet != null)
        {
            bullet.Initialize(rangeDamage, "Player");
        }
        Destroy(sPrefab, 5f);
    }

    public override void Flashbang()
    {
        // Implementar efecto de flashbang si es necesario
        StartCoroutine(FlashbangRoutine());
    }
    public override void FireAttack()
    {
        // Implementar efecto de fuego si es necesario
        StartCoroutine(FireRoutine());
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
    public override void ConsumeRangeAttack()
    {
        if (attacksRecieved <= 0 || !canRangeAttack) return;

        attacksRecieved--;
        canRangeAttack = false;
        StartCoroutine(RangeAttackCooldown());
    }

    public override int NAttacksRecieved()
    {
        return attacksRecieved;
    }
    public override bool CanReciveAttacks()
    {
        return canReciveAttacks;
    }
    public override void SetCanReciveAttacks(bool b)
    {
        canReciveAttacks = b;
    }

    public override bool CanDoRangeAttack()
    {
        return canRangeAttack && attacksRecieved > 0;
    }

    public override Vector3 GetRandomPointInsideBox(BoxCollider box)
    {
        Vector3 center = box.bounds.center;
        Vector3 size = box.bounds.size;

        float x = Random.Range(center.x - size.x / 2f, center.x + size.x / 2f);
        float y = transform.position.y; // Mantener la misma altura del enemigo
        float z = Random.Range(center.z - size.z / 2f, center.z + size.z / 2f);

        return new Vector3(x, y, z);
    }

    private IEnumerator RangeAttackCooldown()
    {
        yield return new WaitForSeconds(0.7f);
        RangeAttackPlayer();
        yield return new WaitForSeconds(rangeAttackCooldown);
        canRangeAttack = true;
    }

    private IEnumerator FlashbangRoutine()
    {
        float flashInitialIntensity = 1000000f;
        float flashDuration = 2f;
        float flashRange = 20f;
        // Instanciar luz
        flashbangLightPrefab.SetActive(true);
        Light flash = flashbangLightPrefab.GetComponent<Light>();

        flash.intensity = flashInitialIntensity;
        flash.range = flashRange;

        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / flashDuration;
            flash.intensity = flashInitialIntensity * Mathf.Exp(-t * 6f);

            yield return null;
        }

        flash.intensity = 0f;
        flashbangLightPrefab.SetActive(false);
    }

    private IEnumerator FireRoutine()
    {
        GetAgent().isStopped = true;
        yield return new WaitForSeconds(1f);
        GetAgent().isStopped = false;
        GameObject[] fire = new GameObject[10];
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = GetRandomPointInsideBox(boxCollider);
            fire[i] = Instantiate(firePrefab, randomPoint, Quaternion.identity);
            fire[i].SetActive(true);
            PrefabDamage firepre = fire[i].GetComponent<PrefabDamage>();
            if (firepre != null)
            {
                firepre.Initialize(rangeDamage, "Player");
            }
            Destroy(fire[i], Random.Range(3f, 5f));
            yield return new WaitForSeconds(0.5f);
        }
        GetAgent().isStopped = false;
    }

}
