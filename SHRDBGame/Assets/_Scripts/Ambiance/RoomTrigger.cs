using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField]
    public string roomName;
    [SerializeField]
    LayerMask interactorLayer;
    public List<Door> door;
    private bool playerInRoom;
    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & interactorLayer) != 0)
        {
            UIManager.Instance.ShowRoomText(roomName);
        }

        if (other.CompareTag("Player"))
        {
            playerInRoom = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & interactorLayer) != 0)
        {
            UIManager.Instance.HideRoomText(roomName);
        }
        if (other.CompareTag("Player"))
        {
            playerInRoom = false;
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
            }
        }

    }
    public bool IsPlayerInRoom()
    {
        return playerInRoom;
    }
}
