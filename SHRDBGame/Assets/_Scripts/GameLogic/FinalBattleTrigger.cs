using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBattleTrigger : MonoBehaviour
{

    public List<Door> door;
    private bool playerInRoom;
    private LevelManager levelManager;
    private bool canStartFinalBattle = true;

    [SerializeField]
    GameObject cardDropPrefab;
    
    public void Start()
    {
        levelManager = FindAnyObjectByType<LevelManager>();

        cardDropPrefab.GetComponent<CardDrop>().RandomCardOnStart = false;
        cardDropPrefab.GetComponent<CardDrop>().CardNameOnStart = "InfiniteKnifeCard";
    }

    void OnTriggerEnter(Collider other)
    {
        if(canStartFinalBattle)
        if (other.CompareTag("Player"))
        {
            playerInRoom = true;

            if(levelManager != null)
            {
                if(levelManager.CurrentNigh >= 5)
                {
                    canStartFinalBattle = false;//spawneaba de forma infinita cartas
                    CerrarPuertas();
                    SpawnCard();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            playerInRoom = false;
        }
    }

    public void AbrirPuertas()
    {
        if (door != null)
        {
            for (int i = 0; i < door.Count; i++)
            {
                if (door[i].isOpen == false)
                {
                    door[i].RotateDoor(transform.position);
                }
            }
        }
    }
    public void CerrarPuertas()//puedes poner directamente lock door que ya se asegura de cerrar las puertas
    {
        if (door != null)
        {
            for (int i = 0; i < door.Count; i++)
            {
                // if (door[i].isOpen == true)
                // {
                //     door[i].RotateDoor(transform.position);
                // }
                door[i].LockDoor();
            }
        }
    }

    public bool IsPlayerInRoom()
    {
        return playerInRoom;
    }

    private void SpawnCard()
    {
        InstantiateCard(cardDropPrefab);
    }

    private void InstantiateCard(GameObject prefab)
    {
            Vector3 spawnPos = GetRandomPointInBox(GetComponent<BoxCollider>());

            spawnPos += Vector3.up * 8;
            GameObject prefabInstantiated = Instantiate(prefab, transform);
            prefabInstantiated.transform.position = spawnPos;
            prefabInstantiated.transform.localEulerAngles = new Vector3(0, 90, 0);

    }
    Vector3 GetRandomPointInBox(BoxCollider box)
    {
        Vector3 size = box.size;
        Vector3 center = box.center;

        // punto aleatorio dentro del volumen del collider (en espacio local)
        Vector3 randomLocalPos = new Vector3(
            UnityEngine.Random.Range(-size.x * 0.5f, size.x * 0.5f),
            UnityEngine.Random.Range(-size.y * 0.5f, size.y * 0.5f),
            UnityEngine.Random.Range(-size.z * 0.5f, size.z * 0.5f)
        );

        // convertimos de espacio local del collider a mundo
        return box.transform.TransformPoint(center + randomLocalPos);
    }
}
