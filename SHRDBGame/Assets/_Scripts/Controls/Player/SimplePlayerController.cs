using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
public class SimplePlayerController : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    private Vector2 inputDir = Vector2.zero;
    public Vector2 Direction{get=>inputDir;}
    private Vector2 inputLook = Vector2.zero;

    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        LookForInput();
        SettingsManager.Instance.onSettingsChange.AddListener(onSettingsChange);
        onSettingsChange();
        GetComponent<Animator>().speed=0f;
    }
    void onSettingsChange()
    {
        speed = SettingsManager.Instance.GetValue<float>("PlayerSpeed");
        
    }
    public void LookForInput()
    {
        Debug.Log("BuscandoInputManager");
        PlayerInput input = InputManager.Instance.Input;
        if (input != null)
        {
            if (GameManager.Instance.gamePlatform == GamePlatform.WebGL_Mobile)
            {
                GameObject.Find("MobileHUD/LeftStick").GetComponent<Joystick>().onMove.AddListener(OnMoveMobile);
            }
            else
            {
                
            
            input.actions["Move"].started += OnMove;
            input.actions["Move"].performed += OnMove;
            input.actions["Move"].canceled += OnMove;
            }

            Debug.Log("InputManager encontrado");
        }
    }
    public void OnMoveMobile(Vector3 dir)
    {
        inputDir=dir;
         GetComponent<Animator>().speed=1.25f;
        if (inputDir.magnitude < 0.001)
        {
              GetComponent<Animator>().speed=0f;
        }

    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        inputDir = ctx.ReadValue<Vector2>();

        GetComponent<Animator>().speed=1.25f;
        
        if (ctx.canceled)
        {
            GetComponent<Animator>().speed=0f;
        }
    }
    public void OnLook(InputAction.CallbackContext ctx)
    {
        inputLook = ctx.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        _smoothedMovementInput = Vector2.SmoothDamp(
        _smoothedMovementInput,
        inputDir,
        ref _movementInputSmoothVelocity,
        0.1f);
        float realSpeed = speed * GetComponent<PlayerCombat>().stats.SpeedMultiplier;
        rb.velocity = new Vector3(_smoothedMovementInput.x * realSpeed, rb.velocity.y, _smoothedMovementInput.y * realSpeed);

        Vector3 lookDir = new Vector3(inputLook.x, 0, inputLook.y);

if (lookDir.sqrMagnitude > 0.0001f)  // evita el warning
{
    transform.rotation = Quaternion.LookRotation(lookDir);
}
    }
    private void OnDestroy()
    {
           PlayerInput input = InputManager.Instance.Input;
        if (input != null)
        {
            input.actions["Move"].started -= OnMove;
            input.actions["Move"].performed -= OnMove;
            input.actions["Move"].canceled -= OnMove;

            Debug.Log("InputManager encontrado");
        }
    }

}
