using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabGift : MonoBehaviour , IInteractable
{
    private bool _isInterctable = true;
    public bool IsInteractable => _isInterctable;

    public string GetInteractionText()
    {
        return "Pulsa E para coger el objeto";
    }

    public Transform GetTransform()
    {
        return this.transform;
    }

    public void Interact()
    {
        PlayerInventory inv = FindObjectOfType<PlayerInventory>();
        inv.CogerObjeto();
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
        }
    }

    public void SetInteractable(bool value)
    {
        _isInterctable = value;
    }
}
