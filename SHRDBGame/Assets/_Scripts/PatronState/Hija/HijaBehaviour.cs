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
        throw new System.NotImplementedException();
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
