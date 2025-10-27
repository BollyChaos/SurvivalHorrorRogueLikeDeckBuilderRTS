using System;
using UnityEngine;
using UnityEngine.InputSystem; // Necesario para PlayerInput

public class LookAtMouseOrGamepad : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask floorLayer;

    private Camera mainCamera;
    private Vector2 lookInput; // se rellena desde PlayerInput

   
    private void Start()
    {
        mainCamera = Camera.main;
        if(Gamepad.current != null)
        {
            LookForInput();
        }

    }

    private void LookForInput()
    {

        PlayerInput input = InputManager.Instance.Input;
        if (input != null)
        {

            input.actions["Look"].started += OnLook;
            input.actions["Look"].performed += OnLook;

        }

    }

    void FixedUpdate()
    {
        if (Gamepad.current != null && lookInput != Vector2.zero) // Si hay mando conectado
        {
            HandleGamepadLook();
        }
        else if(Gamepad.current==null)// fallback al rat�n
        {
            HandleMouseLook();
        }
    }

    private void HandleMouseLook()
    {
        if (InputManager.Instance.inputMap == InputManager.InputMap.UI) return;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, floorLayer))
        {
            Vector3 lookPoint = hit.point;
            lookPoint.y = target.position.y;
            target.LookAt(lookPoint);
        }
    }

    private void HandleGamepadLook()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 screenPos = screenCenter + lookInput * 300f; 

        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, floorLayer))
        {
            Vector3 lookPoint = hit.point;
            lookPoint.y = target.position.y;
            target.LookAt(lookPoint);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
}
