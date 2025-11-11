using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Couch : MonoBehaviour,IInteractable
{
    public bool IsInteractable { get => interactable; }
    bool interactable = true;
    public Transform GetTransform()
    {
        return this.transform;
    }

    public void Interact()
    {
        Debug.Log("Poniendo noche siguiente");
        if (interactable)
        {
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
