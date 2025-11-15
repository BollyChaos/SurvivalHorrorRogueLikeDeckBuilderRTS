using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Obsolete("Sistema obsoleto de prueba")]
public class Couch : MonoBehaviour,IInteractable
{
    public bool IsInteractable { get => interactable; }
    bool interactable = true;
    public Transform GetTransform()
    {
        return this.transform;
    }
public string GetInteractionText()
    {
        return "Presiona E para avanzar la siguiente noche";
    }
    public void Interact()
    {
        if (interactable)
        {
        Debug.Log("Poniendo noche siguiente");

            LevelManager.Instance.NextNight();
            interactable = false;
        }
    }

    public void SetInteractable(bool value)
    {
        if (interactable) 
        Debug.Log("Presiona E para pasar a la siguiente noche");
    }

    // Start is called before the first frame update
    void Start()
    {
        LevelManager.Instance.onNightStateChanged.AddListener(onNightStateChange);
    }
    void onNightStateChange(bool isNight)
    {
        interactable = !isNight;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
