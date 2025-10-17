using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] public bool isInteracting = false;

    void Start()
    {
        LookForInput();
    }

    public void LookForInput()
    {
        Debug.Log("BuscandoInputManager");
        PlayerInput input = InputManager.Instance.Input;
        if (input != null)
        {
            input.actions["Interact"].performed += OnInteract;
            input.actions["Interact"].canceled += OnInteract;

            Debug.Log("InputManager encontrado");
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        isInteracting = context.ReadValue<float>() > 0;
        
    }

    void Update()
    {
        
    }
}
