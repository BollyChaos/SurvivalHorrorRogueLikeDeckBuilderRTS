using UnityEngine;
using UnityEngine.EventSystems;

public class CursorUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.Instance.SetHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instance.SetBase();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CursorManager.Instance.SetClick();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CursorManager.Instance.SetHover();
    }
}
