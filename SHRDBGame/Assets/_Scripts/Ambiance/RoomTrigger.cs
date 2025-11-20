using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField]
    string roomName;
    [SerializeField]
    LayerMask interactorLayer;
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
}
