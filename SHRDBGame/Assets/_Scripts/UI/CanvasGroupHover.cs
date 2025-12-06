using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CanvasGroupHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] CanvasGroup group;
    [SerializeField] float normalAlpha = 0f;
    [SerializeField] float hoverAlpha = 1f;
    [SerializeField] float fadeTime = 0.2f;

    Coroutine fadeRoutine;
public void Awake()
    {
       
        group.alpha = normalAlpha;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartFade(hoverAlpha);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartFade(normalAlpha);
    }

    void StartFade(float target)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(Fade(target));
    }

    IEnumerator Fade(float target)
    {
        float start = group.alpha;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, target, t / fadeTime);
            yield return null;
        }

        group.alpha = target;
    }
}
