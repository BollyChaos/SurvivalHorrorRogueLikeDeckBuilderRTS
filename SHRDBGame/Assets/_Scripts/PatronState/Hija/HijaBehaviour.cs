using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HijaBehaviour : MonoBehaviour, IInteractable
{
    //atributos
    private bool _isInterctable = true;
    public bool IsInteractable => _isInterctable;
    
    

    public string GetInteractionText()
    {
        return "Pulsa E para hablar con la Hija";
    }

    public Transform GetTransform()
    {
        return this.transform;
    }

    public void Interact()
    {
        HijaController hija = GetComponent<HijaController>();
        PlayerInventory inv = FindObjectOfType<PlayerInventory>();
        if (hija.IsTalkable())
        {
            // Lógica para iniciar conversación con la Hija
            Debug.Log("Iniciando conversación con la Hija...");
            if (hija.IsWaitingForGift() && inv.tieneObjeto)
        {
            // Lógica para entregar el objeto a la Hija
            inv.SoltarObjeto();
            hija.SetWaitingForGift(false);
            hija.GiftReceived();
            Debug.Log("Objeto entregado a la Hija.");
        }
        }
        
    }

    public void SetInteractable(bool value)
    {
        if(GetComponent<HijaController>().IsTalkable()){
        _isInterctable = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
