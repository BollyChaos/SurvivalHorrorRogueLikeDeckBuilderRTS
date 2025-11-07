using UnityEngine;

public class PopUp : MonoBehaviour
{
    [Header("Parámetros de animación")]
    [SerializeField] private float popupDuration = 1f;
    [SerializeField] private float rotationDegrees = 720f;
    [SerializeField] private float riseHeight = 0.5f;
    [SerializeField] private float riseDuration = 0.8f;
    [SerializeField] private bool activateOnEnable = true;
    private Vector3 initialScale;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private bool isAnimatingOut = false;

    public void SetInitialTransform()
    {
        initialScale = transform.localScale;
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void OnEnable()
    {
        if (!activateOnEnable) return;
        ActivateAnimation();
    }
    void ActivateAnimation()
    {
         // Cancelar cualquier animación previa de salida
        LeanTween.cancel(gameObject);
        isAnimatingOut = false;

        // Reset de estado inicial
        transform.localScale = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // Animación de aparición (popup)
        LeanTween.scale(gameObject, initialScale, popupDuration)
            .setEaseOutBack();

        LeanTween.rotateAround(gameObject, Vector3.up, rotationDegrees, popupDuration * 1.2f)
            .setEaseOutCubic();

        LeanTween.moveY(gameObject, initialPosition.y + riseHeight, riseDuration)
            .setEaseOutSine();
    }

    void OnDisable()
    {
        if (isAnimatingOut) return;
        isAnimatingOut = true;

        // Cancelar animaciones activas
        LeanTween.cancel(gameObject);

        // Animación inversa (desaparición)

    }
    public void Hide()
    {
        if (isAnimatingOut) return;
        isAnimatingOut = true;

        // Cancelar animaciones activas
        LeanTween.cancel(gameObject);

        // Animación inversa (desaparición)
        LTDescr scaleTween = LeanTween.scale(gameObject, Vector3.zero, popupDuration * 0.8f)
            .setEaseInBack();

        LeanTween.moveY(gameObject, initialPosition.y, riseDuration * 0.8f)
            .setEaseInSine();

        LeanTween.rotate(gameObject, initialRotation.eulerAngles, popupDuration)
            .setEaseInOutCubic();

        // Esperar al final para desactivar el objeto
        scaleTween.setOnComplete(() =>
        {
            transform.localScale = initialScale;
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            isAnimatingOut = false;
            gameObject.SetActive(false);
        });
    }
}
