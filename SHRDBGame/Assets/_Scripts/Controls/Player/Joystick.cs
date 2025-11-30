using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI References")]
    public RectTransform background;
    public RectTransform handle;

    [Header("Settings")]
    public float radius = 100f;

    public Vector2 Direction { get; private set; } = Vector2.zero;
public UnityEvent<Vector3> onMove;
    Vector2 startPosition;

    void Start()
    {
        startPosition = background.anchoredPosition;
        ResetHandle();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        UpdateJoystick(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdateJoystick(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Direction = Vector2.zero;
        onMove.Invoke(Direction);
        ResetHandle();
    }

    void UpdateJoystick(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        // Convertimos a dirección entre -1 y 1
        Vector2 clamped = Vector2.ClampMagnitude(pos, radius);
        Direction = clamped / radius;
        onMove.Invoke(Direction);
        // Mover el handle
        handle.anchoredPosition = clamped;
    }

    void ResetHandle()
    {
        handle.anchoredPosition = Vector2.zero;
    }
     public Vector3 GetMove()
    {
        return new Vector3(Direction.x, 0f, Direction.y);
    }
    void OnDisable()
    {
        ResetHandle();
        onMove.Invoke(Vector2.zero);
    }
}
