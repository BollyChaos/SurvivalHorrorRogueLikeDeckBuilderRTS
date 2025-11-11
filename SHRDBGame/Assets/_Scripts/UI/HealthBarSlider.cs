using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarSlider : MonoBehaviour
{
    [Header("Slider principal")] 
    [SerializeField] private Slider healthSlider;

    [Header("Rellenos")]
    [SerializeField] private Image fillInstant;
    [SerializeField] private Slider fillDelayed;

    [Header("Colores")]
    [SerializeField] private Gradient healthGradient; // Verde → Amarillo → Rojo
    [SerializeField] private Color delayedColor = Color.yellow;

    [Header("Animacion")]
    [SerializeField] private LeanTweenType easeType = LeanTweenType.easeOutCubic;
    [SerializeField] private float delaySpeed = 0.5f;
    [SerializeField] private float initialDelay = 0.15f;

   
    
    private LTDescr delayedTween;
    private void Awake()
    {
        fillDelayed.GetComponentInChildren<Image>().color = delayedColor;
    }
   
     public void SetHealth(float normalizedHealth)
    {
        normalizedHealth = Mathf.Clamp01(normalizedHealth);

        // Cancelar tween previo
        if (delayedTween != null)
            LeanTween.cancel(delayedTween.id);

        // Barra instantánea
        fillInstant.fillAmount = normalizedHealth;
        fillInstant.color = healthGradient.Evaluate(normalizedHealth);
        healthSlider.value = normalizedHealth;

        // Barra retrasada: solo baja si es menor que el actual, sube inmediatamente
        if (normalizedHealth < fillDelayed.value)
        {
            delayedTween = LeanTween.value(fillDelayed.gameObject, fillDelayed.value, normalizedHealth, delaySpeed)
                .setDelay(initialDelay)
                .setEase(easeType)
                .setOnUpdate((float val) =>
                {
                    fillDelayed.value = val;
                });
        }
        else
        {
            fillDelayed.value = normalizedHealth; // si sube, actualizamos de golpe
        }
    }
}
