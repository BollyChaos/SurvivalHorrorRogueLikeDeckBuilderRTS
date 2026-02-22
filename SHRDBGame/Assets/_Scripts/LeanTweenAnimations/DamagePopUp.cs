using TMPro;
using UnityEngine;


public class DamagePopup : MonoBehaviour, IPoolableObject
{
    [Header("Refs")]
    [SerializeField] TextMeshProUGUI tmp;

    [Header("Animation")]
    [SerializeField] float floatY = 1.5f;
    [SerializeField] float duration = 0.8f;
    [SerializeField] float scaleMul = 1.1f;
    [SerializeField]float randomRadius=3.2f;
    [SerializeField] AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Color")]
    [SerializeField] Gradient damageGradient;
    [SerializeField] float maxDamageForGradient = 100f;

    RectTransform rect;
    Vector3 startLocalPos;
    Vector3 startLocalScale;

    public GameObjectPool GameObjectPool { get => gameObjectPool; set => gameObjectPool = value; }
    private GameObjectPool gameObjectPool;

    void Awake()
    {
        rect = transform as RectTransform;
        startLocalScale = rect.transform.localScale;
    }

    [ContextMenu("Play Animation")]
    public void PlayTest()
    {
        Play(50, transform, Vector3.zero);
    }
    public void Play(
        int damage,
        Transform parent,
        Vector3 localPosition)
    {

        // parent primero
        transform.SetParent(parent, false);

        // posición local
         // Reset de estado inicial
        transform.localScale = Vector3.zero;
        Vector3 random=new Vector3(Mathf.Cos(Random.Range(0,2*Mathf.PI)),0,Mathf.Sin(Random.Range(0,2*Mathf.PI)))*randomRadius;
        transform.localPosition = localPosition+random;
        startLocalPos = localPosition;

        // texto
        tmp.text = damage.ToString();

        // color por gradiente
        float t = Mathf.Clamp01(damage / maxDamageForGradient);
        tmp.color = damageGradient.Evaluate(t);

        // reset estado visual
        transform.localScale = startLocalScale;
        tmp.alpha = 1f;

        gameObject.SetActive(true);

        PlayTween();
    }

    void PlayTween()
    {
        LeanTween.cancel(gameObject);

        // movimiento vertical local
        LeanTween.moveLocalY(gameObject, startLocalPos.y + floatY, duration)
            .setEase(ease);

        // fade
        LeanTween.value(gameObject, 1f, 0f, duration)
            .setOnUpdate((float a) => tmp.alpha = a);

        // pequeña escala punch opcional
        LeanTween.scale(gameObject, Vector3.one * scaleMul, duration * 0.3f)
            .setEaseOutBack()
            .setLoopPingPong(1);

        // release automático
        LeanTween.delayedCall(gameObject, duration, Release);
    }

    void Release()
    {
        LeanTween.cancel(gameObject);
        if (gameObjectPool != null)
            gameObjectPool.Release(gameObject);
    }

    void IPoolableObject.Release()
    {
        Release();
    }
}