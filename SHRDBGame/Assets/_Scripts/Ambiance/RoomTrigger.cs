using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField]
    public string roomName;
    [SerializeField]
    LayerMask interactorLayer;
    public Door door;
    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & interactorLayer) != 0)
        {
            UIManager.Instance.ShowRoomText(roomName);
        }

    }
    void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & interactorLayer) != 0)
        {
            UIManager.Instance.HideRoomText(roomName);
        }
    }
    public void CerrarPuerta()
    {
        door.RotateDoor(transform.position);
    }
}
