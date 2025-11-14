using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCardAction : MonoBehaviour, ICardAction
{
    // Start is called before the first frame update
    private enum TeleportState { SETSPAWN, TELEPORT }
    [SerializeField]
    private TeleportState teleportState = TeleportState.SETSPAWN;


    [SerializeField]
    private Transform playerTransform;
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
    [SerializeField] GameObject teleportSpawnPrefab;

    [SerializeField] GameObject teleportPrefab;

    [SerializeField]
    private Vector3 teleportPosition;
    [SerializeField] GameObject teleportspawnprefab;

    // // 🔊 NUEVO: sonidos (2D) te lo voy a seguir comentando hasta que lo hagas bien
    // [SerializeField] private AudioClip placeTeleportSound;
    // [SerializeField] private AudioClip teleportSound;
    // [SerializeField, Range(0f, 1f)] private float teleportSoundVolume = 1f;

    // AudioSource local (usado para reproducir en 2D)
    // private AudioSource audioSource;

    // void Awake()
    // {
    //     // asegúrate de tener un AudioSource local (2D)
    //     audioSource = GetComponent<AudioSource>();
    //     if (audioSource == null)
    //     {
    //         audioSource = gameObject.AddComponent<AudioSource>();
    //         audioSource.playOnAwake = false;
    //         audioSource.spatialBlend = 0f; // 2D
    //     }
    // }

    void Start()
    {
        if (teleportSpawnPrefab != null) teleportSpawnPrefab.SetActive(false);
        if (teleportPrefab != null) teleportPrefab.SetActive(false);
    }

    public void ExecuteCardAction(CardObject cardObj)
    {
        //comprobacion por sea caso se queda la carta pillada(el jugador se muere habiendo puesto un portal y en la siguiente partida se queda guardado ese punto)
        if (cardObj.CardNUses == 2) { Reset(); }
        switch (teleportState)
        {
            case TeleportState.SETSPAWN:
                SetSpawn();
                break;
            case TeleportState.TELEPORT:
                Teleport();
                break;
        }

        cardObj.UsingCard = false;
    }

    void SetSpawn()
    {
        teleportPosition = playerTransform.position;
        teleportspawnprefab = Instantiate(teleportSpawnPrefab, playerTransform.position, Quaternion.identity);
        teleportspawnprefab.SetActive(true);
        teleportspawnprefab.GetComponent<ParticleSystem>().Play();
        teleportState = TeleportState.TELEPORT;

        // // 🔊 Sonido al colocar el portal (2D)
        // if (placeTeleportSound != null && audioSource != null)
        //     audioSource.PlayOneShot(placeTeleportSound, teleportSoundVolume);
        GetComponent<ASoundPlayer>().PlaySound(0);
    }

    void Teleport()
    {
        StartCoroutine(TeleportRoutine());
    }

    IEnumerator TeleportRoutine()
    {
        // Efecto de salida
        var teleportprefab = Instantiate(teleportPrefab, playerTransform.position, Quaternion.identity);
        teleportprefab.SetActive(true);
        teleportprefab.GetComponent<ParticleSystem>().Play();
        Destroy(teleportprefab, 0.5f);

        // // 🔊 Sonido de teletransporte (2D)
        // if (teleportSound != null && audioSource != null)
        //     audioSource.PlayOneShot(teleportSound, teleportSoundVolume);
        GetComponent<ASoundPlayer>().PlaySound(1);

        var rb = playerTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            yield return new WaitForFixedUpdate(); // esperar al paso de física
            rb.MovePosition(teleportPosition); // teletransportar correctamente
        }

        yield return null;
        Reset();
    }

    void Reset()
    {
        teleportState = TeleportState.SETSPAWN;
        teleportPosition = Vector3.zero;
        if (teleportspawnprefab != null)
            Destroy(teleportspawnprefab, 0.5f);
    }
}
