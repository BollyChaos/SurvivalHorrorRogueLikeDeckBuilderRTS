using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBattleTrigger : MonoBehaviour
{

    public List<Door> door;
    private bool playerInRoom;
    private LevelManager levelManager;

    [SerializeField]
    GameObject cardDropPrefab;
    
    public void Start()
    {
        levelManager = FindAnyObjectByType<LevelManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRoom = true;

            if(levelManager != null)
            {
                if(levelManager.CurrentNigh >= 5)
                {
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
    public void CerrarPuertas()
    {
        if (door != null)
        {
            for (int i = 0; i < door.Count; i++)
            {
                if (door[i].isOpen == true)
                {
                    door[i].RotateDoor(transform.position);
                }
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
        InstantiateCard(cardDropPrefab, 1);
    }

    private void InstantiateCard(GameObject prefab, int nDrops)
    {
        Vector3 spawnPos = new Vector3(3.6400001f,0,120.779999f);
            spawnPos += Vector3.up * 8;
            GameObject prefabInstantiated = Instantiate(prefab, transform);
            prefabInstantiated.transform.localPosition = spawnPos;
            prefabInstantiated.transform.localEulerAngles = new Vector3(0, 90, 0);

    }
}
