using UnityEngine;

public class IdleFloatAndRotate : MonoBehaviour
{
    [Header("Rotación")]
    [SerializeField] private float rotationSpeed = 30f; // grados por segundo

    [Header("Flotación")]
    [SerializeField] private float floatAmplitude = 0.25f;
    [SerializeField] private float floatDuration = 1.5f;
    [SerializeField]public bool activateOnEnable = true;

    private Vector3 startPosition;
    private LTDescr floatTween;
    private LTDescr rotateTween;
    public void OnEnable()
    {
        rotationSpeed=Random.Range(rotationSpeed-floatAmplitude,rotationSpeed+floatAmplitude);
        if (activateOnEnable)
            ActivateAnimation();
    }
    public void OnDisable()
    {
        DeActivateAnimation();
    }

    public void ActivateAnimation()
    {
        startPosition = transform.position;

        // Rotación continua en eje Y
        rotateTween = LeanTween.rotateAround(gameObject, Vector3.up, 360f, 360f / rotationSpeed)
            .setLoopClamp(); // rotación infinita

        // Movimiento vertical tipo “flotante” (ping-pong)
        floatTween = LeanTween.moveY(gameObject, startPosition.y + floatAmplitude, floatDuration)
            .setEaseInOutSine()
            .setLoopPingPong();

    }
    public void DeActivateAnimation()
    {
        // Cancelar animaciones al desactivar el objeto
        LeanTween.cancel(gameObject);
        transform.position = startPosition; // opcional: resetear posición
        transform.rotation = Quaternion.identity; // opcional: resetear rotación
    }
}
