using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class HijaController : EnemyController
{
    //atributos
    private bool talkable = true;
    public bool _salonAbierto = false;
    private bool _missionsCompleted = false;
    private float damage = 50;
    private float rangeDamage = 20f;
    [SerializeField] GameObject slashPrefab;
    [SerializeField] GameObject BulletPrefab;
    [SerializeField] int nMisions = 3;
    [SerializeField] GameObject _player;
    private bool Crying = false;
    public bool waitingForGift = false;

    [Header("Utility System")]
    public float timeSinceGift = 0f;  // TSR
    public float timeSinceSeen = 0f;  // TSJ

    public float enfado = 0f;
    public float aburrimiento = 0f;
    public float ganasDeLlorar = 0f;

    [SerializeField] public float enfadoMax = 100f;
    [SerializeField] public float aburrimientoMax = 100f;
    [SerializeField] public float llorarUmbral = 0.6f;

    private bool playerInRoom = false;

    [SerializeField] private ASoundPlayer soundPlayer;
    private bool enfadoSoundPlayed = false;

    public void Awake()
    {
        base.Awake();
        _player = GameObject.FindGameObjectWithTag("Player");
        SetState(new HijaWaiting(this));

        if (soundPlayer == null)
            soundPlayer = GetComponent<ASoundPlayer>();
    }

    void Start() { }

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
        if (soundPlayer != null)
            soundPlayer.PlaySound(0);

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
        if (soundPlayer != null)
            soundPlayer.PlaySound(1);

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
    #endregion

    #region player
    public override GameObject GetPlayer()
    {
        return _player;
    }

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

    #region regalo
    public override bool IsWaitingForGift()
    {
        return waitingForGift;
    }
    public override void SetWaitingForGift(bool estado)
    {
        waitingForGift = estado;
    }
    public override void GiftReceived()
    {
        timeSinceGift = 0f;
    }
    #endregion

    public void OnReset()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        talkable = true;
        _salonAbierto = false;
        _missionsCompleted = false;
        Crying = false;
        waitingForGift = false;
        timeSinceGift = 0f;
        timeSinceSeen = 0f;
        enfado = 0f;
        aburrimiento = 0f;
        ganasDeLlorar = 0f;
        enfadoSoundPlayed = false;

        SetState(new HijaWaiting(this));
    }

    public void ChangePosition(Vector3 newPosition)
    {
        GetAgent().Warp(newPosition);
    }
}