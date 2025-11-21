using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HijaController : EnemyController
{
    //atributos
    private bool talkable = true;
    private bool _salonAbierto = false;
    private bool _missionsCompleted = false;
    private float damage = 50;
    private float rangeDamage = 20f;
    [SerializeField] GameObject slashPrefab;
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] int nMisions = 3;
    [SerializeField] GameObject _player;
    private bool Crying = false;
    [Header("Utility System")]
    public float timeSinceGift = 0f;  // TSR
    public float timeSinceSeen = 0f;  // TSJ

    public float enfado = 0f;
    public float aburrimiento = 0f;
    public float ganasDeLlorar = 0f;

    [SerializeField] public float enfadoMax = 10f;
    [SerializeField] public float aburrimientoMax = 10f;
    [SerializeField] public float llorarUmbral = 0.6f;
    // Estado del trigger del salón
    private bool playerInRoom = false;

    //metodos
    public void Awake()
    {
        base.Awake();
        SetState(new HijaBattling(this));
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    #region salon
    public override void SetSalonAbierto(bool estado)
    {
        _salonAbierto = estado;
    }
    public override bool IsSalonAbierto()
    {
        return _salonAbierto;
    }
    #endregion

    #region Hablar
    public override void SetTalkable(bool estado)
    {
        talkable = estado;
    }
    public override bool IsTalkable()
    {
        return talkable;
    }
    #endregion

    #region misiones
    public override void SetMisionsCompleted(bool estado)
    {
        _missionsCompleted = estado;
    }
    public override bool AreMisionsCompleted()
    {
        return _missionsCompleted;
    }
    #endregion
    #region ataque
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
        ///Se Crea el slash para que el enemigo ataque
        GameObject sPrefab = Instantiate(BulletPrefab, transform.position + transform.forward * 2, transform.rotation);
        sPrefab.SetActive(true);
        ParticleSystem ps = sPrefab.GetComponent<ParticleSystem>();
        ps.Play();

        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(rangeDamage, "Player");
        }
        Destroy(sPrefab, 5f);
    }
    #endregion
    #region player
    public override GameObject GetPlayer()
    {
        return _player;
    }
    // Se llama desde trigger del salón
    public override void PlayerEnteredRoom()
    {
        playerInRoom = true;
        timeSinceSeen = 0f;
    }

    public override void PlayerLeftRoom()
    {
        playerInRoom = false;
    }
    public override bool IsPlayerInRoom()
    {
        return playerInRoom;
    }
    #endregion
    public override void SetCrying(bool estado)
    {
        Crying = estado;
    }
    public override bool IsCrying()
    {
        return Crying;
    }
}
