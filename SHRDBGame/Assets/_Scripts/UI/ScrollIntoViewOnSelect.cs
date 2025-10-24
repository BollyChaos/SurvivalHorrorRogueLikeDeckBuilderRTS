using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class ScrollIntoViewOnSelect : MonoBehaviour, ISelectHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        if (!gameObject.activeInHierarchy)
            return;

        StartCoroutine(ScrollToCenter());
    }

    private IEnumerator ScrollToCenter()
    {
        yield return null; // Espera un frame para asegurar layout actualizado

        ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect == null || scrollRect.viewport == null || scrollRect.content == null)
            yield break;

        RectTransform viewport = scrollRect.viewport;
        RectTransform content = scrollRect.content;
        RectTransform selected = GetComponent<RectTransform>();

        // Posición local del item dentro del content
        Vector2 localPos = content.InverseTransformPoint(selected.position);
        Vector2 centerInContent = content.InverseTransformPoint(viewport.position);

        float diffY = localPos.y - centerInContent.y;

        float scrollHeight = content.rect.height - viewport.rect.height;
        if (Mathf.Approximately(scrollHeight, 0f))
            yield break;

        // Calcula nueva posición normalizada
        float normalizedY = scrollRect.verticalNormalizedPosition + (diffY / scrollHeight);
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizedY);
    }
}
