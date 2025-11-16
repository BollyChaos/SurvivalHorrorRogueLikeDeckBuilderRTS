using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public bool IsInteractable =>isInteractable;
    private bool isInteractable=false;

    public string GetInteractionText()
    {
        if(!isInteractable) return "";
        return "Pulsa E para abrir la puerta";
    }

    

    public void Interact()
    {
        //FindAnyObjectByType<SimplePlayerController>().gameObject.transform.position;
    }

    public void SetInteractable(bool value)
    {
    }
    public Transform GetTransform()
    {
        return transform;
    }
}
