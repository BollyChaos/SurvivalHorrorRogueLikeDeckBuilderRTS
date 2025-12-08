using UnityEngine;

public class LookAtBehaviour : MonoBehaviour
{
    [Header("Target a mirar")]
    public Transform target;

    [Header("Rotación")]
    public float rotateSpeed = 0.3f;
    public float minX = -20f;
    public float maxX = 20f;

    private LTDescr _lookTween;
    private Quaternion _startRotation;

    private void Awake()
    {
        _startRotation = transform.localRotation;
    }

    private void OnEnable()
    {
        StartLooking();
    }

    private void OnDisable()
    {
        StopLooking();
    }

    public void StartLooking()
    {
        if (target == null || _lookTween != null)
            return;

        // Loop update
        _lookTween = LeanTween.value(gameObject, 0f, 1f, rotateSpeed)
            .setOnUpdate((float _) => LookUpdate())
            .setLoopClamp();
    }

    public void StopLooking()
    {
        if (_lookTween != null)
        {
            LeanTween.cancel(gameObject);
            _lookTween = null;
            transform.localRotation = _startRotation;
        }
    }

    public void RestartLooking()
    {
        StopLooking();
        StartLooking();
    }

    private void LookUpdate()
    {
        if (target == null)
            return;

        // Dirección hacia el target
        Vector3 direction = (target.position - transform.position).normalized;

        // La rotación deseada
        Quaternion desiredRot = Quaternion.LookRotation(direction);

        // Aplicar clamp solo en X
        Vector3 e = desiredRot.eulerAngles;
        float clampedX = ClampAngle(e.x, minX, maxX);
        desiredRot = Quaternion.Euler(clampedX, e.y, e.z);

        // Rotación suave
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * 5f);
    }

    /// <summary>
    /// Clamp correcto para ángulos que cruzan 0/360.
    /// </summary>
    private float ClampAngle(float angle, float min, float max)
    {
        angle = Mathf.Repeat(angle + 180f, 360f) - 180f;
        return Mathf.Clamp(angle, min, max);
    }
}
