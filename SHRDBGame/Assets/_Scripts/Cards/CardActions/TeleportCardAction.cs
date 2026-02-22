using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportCardAction : ACardAction
{
    //TODO: no destruyas más cosas por favor, si es null instanciar y referenciar, si no set active=true
    // Start is called before the first frame update
    private enum TeleportState { SETSPAWN, TELEPORT }
    [SerializeField]
    private TeleportState teleportState = TeleportState.SETSPAWN;

    [SerializeField] GameObject teleportSpawnPrefab;

    [SerializeField] GameObject teleportPrefab;

    [SerializeField]
    private Vector3 teleportPosition;
    [SerializeField] GameObject teleportspawnprefab;

   

    void Start()
    {
        if (teleportSpawnPrefab != null) teleportSpawnPrefab.SetActive(false);
        if (teleportPrefab != null) teleportPrefab.SetActive(false);
    }

    public override void ExecuteCardAction(CardObject cardObj)
    {
        //comprobacion por sea caso se queda la carta pillada(el jugador se muere habiendo puesto un portal y en la siguiente partida se queda guardado ese punto)
        if (cardObj.CardNUses == 2) { ResetCardAction();//Release(); 
        }
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
        teleportPosition = PlayerTransform.position;
        teleportspawnprefab = Instantiate(teleportSpawnPrefab, PlayerTransform.position, Quaternion.identity);
        teleportspawnprefab.SetActive(true);
        teleportspawnprefab.GetComponent<ParticleSystem>().Play();
        teleportState = TeleportState.TELEPORT;
        GetComponent<ASoundPlayer>().PlaySound(0);
    }

    void Teleport()
    {
        StartCoroutine(TeleportRoutine());
    }

    IEnumerator TeleportRoutine()
    {
        // Efecto de salida
        var teleportprefab = Instantiate(teleportPrefab, PlayerTransform.position, Quaternion.identity);
        teleportprefab.SetActive(true);
        teleportprefab.GetComponent<ParticleSystem>().Play();
        Destroy(teleportprefab, 0.5f);

        GetComponent<ASoundPlayer>().PlaySound(1);

        var rb = PlayerTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            yield return new WaitForFixedUpdate(); // esperar al paso de física
            rb.MovePosition(teleportPosition); // teletransportar correctamente
        }

        yield return null;
        //Release();
        ResetCardAction();
    }

    

    public override void ResetCardAction()
    {
        //aquí set active=false
          teleportState = TeleportState.SETSPAWN;
        teleportPosition = Vector3.zero;
        if (teleportspawnprefab != null)
            Destroy(teleportspawnprefab, 0.5f);
    }

}
