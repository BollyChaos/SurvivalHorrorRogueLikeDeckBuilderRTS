using UnityEngine;

public class CameraBoatMotion : MonoBehaviour
{
    [Header("Movimiento vertical (balanceo)")]
    [SerializeField] private float positionAmplitude = 0.05f;   // Cuánto se mueve arriba/abaj
    [SerializeField] private float positionFrequency = 0.5f;    // Velocidad del movimiento

    [Header("Rotación (balanceo del barco)")]
    [SerializeField] private float rotationAmplitude = 1.5f;    // Grados de inclinación
    [SerializeField] private float rotationFrequency = 0.3f;    // Velocidad del giro

    private Vector3 initialPos;
    private Quaternion initialRot;
    private float timeOffset;

    void Start()
    {
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
        timeOffset = Random.value * 10f; // Para evitar sincronía si hay varias cámaras/objetos
    }

    void Update()
    {
        float t = Time.time + timeOffset;

        // Movimiento vertical suave (sinusoidal)
        float verticalOffset = Mathf.Sin(t * positionFrequency) * positionAmplitude;

        // Rotación suave alrededor de X e Y
        float tiltX = Mathf.Sin(t * rotationFrequency) * rotationAmplitude;
        float tiltZ = Mathf.Cos(t * rotationFrequency * 0.7f) * rotationAmplitude * 0.6f;

        transform.localPosition = initialPos + new Vector3(0, verticalOffset, 0);
        transform.localRotation = initialRot * Quaternion.Euler(tiltX, 0, tiltZ);
    }
}
