using UnityEngine;
using UnityEngine.InputSystem;
public class SimplePlayerController : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    private Vector2 inputDir = Vector2.zero;
    private Vector2 inputLook = Vector2.zero;

    private Vector2 _smoothedMovementInput;
    private Vector2 _movementInputSmoothVelocity;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        LookForInput();
    }
    public void LookForInput()
    {
        Debug.Log("BuscandoInputManager");
        PlayerInput input = InputManager.Instance.Input;
        if (input != null)
        {
            input.actions["Move"].performed += OnMove;
            input.actions["Move"].canceled += OnMove;

            Debug.Log("InputManager encontrado");
        }
    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        inputDir = ctx.ReadValue<Vector2>();
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

        rb.linearVelocity = new Vector3(_smoothedMovementInput.x * speed, rb.linearVelocity.y, _smoothedMovementInput.y * speed);

        transform.rotation = Quaternion.LookRotation(new Vector3(inputLook.x, inputLook.y, 0));
    }

}
