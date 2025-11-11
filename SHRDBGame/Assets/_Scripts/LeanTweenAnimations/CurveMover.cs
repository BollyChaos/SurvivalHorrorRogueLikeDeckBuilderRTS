using UnityEngine;

public class CurveMover : MonoBehaviour
{
    [Header("Movimiento principal")]
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float curveAmplitude = 2f;
    [SerializeField] private Transform dynamicTarget;
    [SerializeField] private float recalcThreshold = 0.5f; // si el target se movió más que esto, se recalcula
    [SerializeField] private float completionDistance = 0.4f; // cuando estamos lo bastante cerca, termina

    [Header("Animación final")]
    [SerializeField] private float riseHeight = 1f;
    [SerializeField] private float riseDuration = 0.3f;
    [SerializeField] private float fallDuration = 0.2f;

    private Vector3 startPos;
    private Vector3 midPoint1;
    private Vector3 midPoint2;
    private Vector3 oldTargetPos;
    private bool isMoving;
    private bool isFinishing;

    private LTDescr currentTween;

    void Update()
    {
        if (!isMoving || isFinishing || dynamicTarget == null) return;

        // Si el target se movió demasiado -> recalcular
        if (Vector3.Distance(oldTargetPos, dynamicTarget.position) > recalcThreshold)
        {
            oldTargetPos = dynamicTarget.position;
            MoveToCurve(dynamicTarget.position);
        }

        // Si ya está cerca del objetivo -> ejecutar animación final
        if (Vector3.Distance(transform.position, dynamicTarget.position) <= completionDistance)
        {
            isMoving = false;
            FinishAnimation();
        }
    }

    public void MoveToCurve(Vector3 targetPosition)
    {
        if (currentTween != null) LeanTween.cancel(gameObject);

        startPos = transform.position;
        oldTargetPos = targetPosition;

        // Puntos intermedios aleatorios para la curva
        midPoint1 = startPos + new Vector3(
            Random.Range(-curveAmplitude, curveAmplitude),
            Random.Range(-curveAmplitude, curveAmplitude),
            0f);

        midPoint2 = startPos + new Vector3(
            Random.Range(-curveAmplitude, curveAmplitude),
            Random.Range(-curveAmplitude, curveAmplitude),
            0f);

        isMoving = true;

        currentTween = LeanTween.value(gameObject, 0f, 1f, duration)
            .setEaseInOutSine()
            .setOnUpdate((float t) =>
            {
                Vector3 currentTarget = dynamicTarget ? dynamicTarget.position : oldTargetPos;
                Vector3 newPos = Bezier4(startPos, midPoint1, midPoint2, currentTarget, t);
                transform.position = newPos;
            })
            .setOnComplete(() => isMoving = false);
    }

    private void FinishAnimation()
    {
        if (isFinishing) return;
        isFinishing = true;

        Vector3 riseTarget = transform.position + Vector3.up * riseHeight;
        Vector3 fallTarget = dynamicTarget ? dynamicTarget.position : transform.position;

        // Subir suavemente
        LeanTween.move(transform.gameObject, riseTarget, riseDuration)
            .setEaseOutQuad()
            .setOnComplete(() =>
            {
                // Bajar rápido hacia el objetivo y desaparecer
                LeanTween.move(transform.gameObject, fallTarget, fallDuration)
                    .setEaseInQuad()
                    .setOnComplete(() =>
                    {
                        gameObject.SetActive(false);
                        isFinishing = false;
                    });
            });
    }

    // Curva de Bézier cúbica
    private Vector3 Bezier4(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        return (u * u * u) * p0 +
               (3 * u * u * t) * p1 +
               (3 * u * t * t) * p2 +
               (t * t * t) * p3;
    }
}
