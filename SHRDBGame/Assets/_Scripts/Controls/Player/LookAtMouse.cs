using System;
using UnityEngine;
using UnityEngine.InputSystem; // Necesario para PlayerInput
using Cinemachine;

public class LookAtMouseOrGamepad : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private LayerMask floorLayer;

    private Camera mainCamera;
    private Vector2 lookInput; // se rellena desde PlayerInput

    [SerializeField] private CinemachineVirtualCamera vcam;
   private CinemachineFramingTransposer framingTransposer;


    private Vector2 offsetCurrent = new Vector2(0.5f, 0.5f);
    private Vector2 offsetVelocity;
    [SerializeField]
    private  float maxOffset = 0.1f;   // máximo +/- 0.1 desde el centro
    [SerializeField]
    private  float springTime = 0.12f; // suavidad del muelle

    private void Start()
    {
        mainCamera = transform.parent.GetComponent<CameraController>().PlayerCamera;

        if (Gamepad.current != null)
        {
            LookForInput();
        }
        enabled = false;
        enabled = true;
    framingTransposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();


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
        else if (Gamepad.current == null)// fallback al rat�n
        {
            HandleMouseLook();
        }
        HandleCameraOffset();

    }
  private void HandleCameraOffset()
{
    if (framingTransposer == null) return;

    // Input normalizado (-1..1)
    
    Vector2 input ;
        if (Gamepad.current == null)
        {
input=GetMouseInputNormalized();
        }
        else
        {
    input= lookInput.normalized;
            
        }

    // Calculamos el target offset según el input y límite máximo
//    Debug.Log(input);
    Vector2 cameraInput=new Vector2(-input.x,input.y);
    Vector2 targetOffset2D = offsetCurrent+cameraInput * maxOffset;
    

    // Suavizamos el offset con un muelle (SmoothDamp)
    Vector2 cameraOffset = Vector2.SmoothDamp(
        offsetCurrent,
        targetOffset2D,
        ref offsetVelocity,
        springTime
    );

    // Aplicamos al FramingTransposer
    framingTransposer.m_ScreenX=cameraOffset.x;
    framingTransposer.m_ScreenY=cameraOffset.y;
    // Vector3 trackedObjectOffset = framingTransposer.m_TrackedObjectOffset;
    // trackedObjectOffset.x = offsetCurrent.x;
    // trackedObjectOffset.y = offsetCurrent.y;

    // framingTransposer.m_TrackedObjectOffset = trackedObjectOffset;
}
private Vector2 GetMouseInputNormalized()
{
    // Obtener posición del ratón en pixeles
    Vector2 mousePos = Input.mousePosition;

    // Centro de la pantalla
    Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

    // Offset desde el centro, normalizado en rango [-1, 1]
    Vector2 normalized = new Vector2(
        Mathf.Clamp((mousePos.x - screenCenter.x) / screenCenter.x, -1f, 1f),
        Mathf.Clamp((mousePos.y - screenCenter.y) / screenCenter.y, -1f, 1f)
    );

    return normalized;
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
    public void OnDestroy()
    {
        PlayerInput input = InputManager.Instance.Input;
        if (input != null)
        {

            input.actions["Look"].started -= OnLook;
            input.actions["Look"].performed -= OnLook;

        }
    }
}
