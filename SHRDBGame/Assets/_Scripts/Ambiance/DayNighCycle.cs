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
    [Tooltip("Rotación total del sol (360° normalmente)")]
    [SerializeField] private float fullRotationDegrees = 360f;
    [Tooltip("Inclinación del sol cuando está en su punto más alto (día)")]
    [SerializeField, Range(0f, 90f)] private float dayInclination = 45f;
    [Tooltip("Inclinación del sol cuando está más bajo (noche)")]
    [SerializeField, Range(-90f, 0f)] private float nightInclination = -15f;

    [Header("Day Control")]
    public bool isDayCondition; // Cambia según el estado del juego

    private bool isDayActive;
    private float transitionTimer;
    private float currentT;
    private float rotationY; // Rotación Y acumulada, se mantiene estable

    void Start()
    {
        if (directionalLight == null)
        {
            Debug.LogError("[DayNightCycle] Falta asignar la luz direccional.");
            enabled = false;
            return;
        }

        // Empieza de noche
        isDayActive = false;
        currentT = 0f;
        rotationY = 0f;
        ApplyLighting(0f);

        LevelManager.Instance.onNightStateChanged.AddListener((isNight) =>
        {
            isDayCondition = !isNight;
        });

        dayTransitionDuration = LevelManager.Instance.NightDuration;
    }

    void Update()
    {
        // Detectar cambio de estado
        if (isDayCondition != isDayActive)
        {
            isDayActive = isDayCondition;
            transitionTimer = 0f;
        }

        // Calcular duración según transición
        float duration = isDayActive ? dayTransitionDuration : nightTransitionDuration;

        // Avanzar transición (solo si no ha terminado)
        if (transitionTimer < duration)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / duration);
            currentT = isDayActive ? t : 1f - t;

            // Durante la transición, actualizar la rotación Y
            rotationY = (currentT * fullRotationDegrees) % 360f;
        }

        ApplyLighting(currentT);
    }

    private void ApplyLighting(float t)
    {
        // Color e intensidad
        Color skyColor = skyGradient.Evaluate(t);
        float intensity = intensityCurve.Evaluate(t) * maxIntensity;

        // Rotación: inclinación interpolada + rotación Y estable
        float inclination = Mathf.Lerp(nightInclination, dayInclination, t);
        directionalLight.transform.rotation = Quaternion.Euler(inclination, rotationY, 0f);

        directionalLight.color = skyColor;
        directionalLight.intensity = intensity;

        RenderSettings.ambientLight = skyColor;
    }
}
