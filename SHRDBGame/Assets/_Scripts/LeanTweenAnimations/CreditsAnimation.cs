using Managers;
using UnityEngine;
using UnityEngine.UI;

public class CreditsAnimation : MonoBehaviour
{
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private float duration = 10f;
    [SerializeField] private CanvasGroup canvasGroup;
    private LTDescr tween;

    private void OnEnable()
    {
        // Reiniciar al inicio
        scroll.verticalNormalizedPosition = 1f;
        canvasGroup.alpha = 1f;

        // Iniciar animación de scroll (1 → 0)
        tween = LeanTween.value(gameObject, 1f, 0f, duration)
     .setOnUpdate((float v) =>
     {
         scroll.verticalNormalizedPosition = v;
     })
     .setOnComplete(() =>
     {
         // Fade out
         LeanTween.value(gameObject, 1f, 0f,5)
             .setOnUpdate((float a) =>
             {
                 canvasGroup.alpha = a;
             })
             .setOnComplete(() =>
             {
                 // Llamada a tu función al finalizar TODO
                 DisappearCredits();
             });
     });


    }
    private void DisappearCredits()
    {
        gameObject.SetActive(false);
        GameManager.Instance.OutCredits();
    }
    private void OnDisable()
    {
        // Cancelar tween si existe
        if (tween != null)
        {
            LeanTween.cancel(gameObject);
            tween = null;
        }

        // Volver al valor inicial
        if (scroll != null)
            scroll.verticalNormalizedPosition = 1f;
    }
}
