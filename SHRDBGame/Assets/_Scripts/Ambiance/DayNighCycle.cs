using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private Light directionalLight;
    [SerializeField] private Gradient skyGradient;
    [SerializeField] private AnimationCurve intensityCurve;
    [SerializeField] private float dayTransitionDuration = 10f;
    [SerializeField] private float nightTransitionDuration = 3f;
    [SerializeField] private float maxIntensity = 1f;

    [Header("Rotación del Sol")]
    [SerializeField] private float fullRotationDegrees = 360f;
    [SerializeField, Range(0f, 90f)] private float dayInclination = 45f;
    [SerializeField, Range(-90f, 0f)] private float nightInclination = -15f;

    [Header("Day Control")]
    public bool isDayCondition; // objetivo actual: día o noche
    private bool startTransition = false; // se activa desde evento
    private float transitionTime; // duración de la transición
    private float currentT;       // 0 = noche, 1 = día
    private float rotationY;

    void Start()
    {
        if (directionalLight == null)
        {
            Debug.LogError("[DayNightCycle] Falta asignar la luz direccional.");
            enabled = false;
            return;
        }

        // Inicializamos según estado actual
        currentT = isDayCondition ? 1f : 0f;
        rotationY = currentT * fullRotationDegrees;
        ApplyLighting(currentT);

        // Suscribirse al evento
        LevelManager.Instance.onNightStateChanged.AddListener(OnNightStateChanged);
    }

    void OnNightStateChanged(bool isNight)
    {
        isDayCondition = !isNight;
        startTransition = true;
        transitionTime = isDayCondition ? dayTransitionDuration : nightTransitionDuration;
    }

    void Update()
    {
        if (startTransition)
        {
            float targetT = isDayCondition ? 1f : 0f;
            currentT = Mathf.MoveTowards(currentT, targetT, Time.deltaTime / transitionTime);

            // Actualizar rotación
            rotationY = currentT * fullRotationDegrees;
            ApplyLighting(currentT);

            if (Mathf.Approximately(currentT, targetT))
            {
                startTransition = false;
            }
        }
    }

    private void ApplyLighting(float t)
    {
        Color skyColor = skyGradient.Evaluate(t);
        float intensity = intensityCurve.Evaluate(t) * maxIntensity;
        float inclination = Mathf.Lerp(nightInclination, dayInclination, t);

        directionalLight.transform.rotation = Quaternion.Euler(inclination, rotationY, 0f);
        directionalLight.color = skyColor;
        directionalLight.intensity = intensity;

        RenderSettings.ambientLight = skyColor;
    }
}
