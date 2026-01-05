using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBattleTrigger : MonoBehaviour
{

    public List<Door> door;
    private bool playerInRoom;
    private LevelManager levelManager;

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
}
