using UnityEngine;

public class MoveUIBetweenCanvases : MonoBehaviour
{
    [Header("Canvases destino y origen")]
    [SerializeField] public Canvas worldCanvas;
    [SerializeField] public Canvas screenCanvas;

    public RectTransform rectTransform;
    public Camera worldCamera;

    /// <summary>
    /// Convierte este objeto del Canvas de World Space al de Screen Space.
    /// </summary>
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    [ContextMenu("Move To Screen Canvas")]
    public void MoveToScreenCanvas()
    {
        // 1️⃣ Guardar posición mundial actual del objeto
        Vector3 worldPos = rectTransform.position;

        // 2️⃣ Convertir la posición mundial a coordenadas de pantalla
        Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPos);

        // 3️⃣ Cambiar el parent al canvas de pantalla
        rectTransform.SetParent(screenCanvas.transform, false);

        // 4️⃣ Convertir las coordenadas de pantalla a coordenadas locales del nuevo canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            screenCanvas.transform as RectTransform,
            screenPos,
            screenCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : screenCanvas.worldCamera,
            out Vector2 localPoint
        );

        // 5️⃣ Asignar posición, rotación y escala coherentes
        rectTransform.localPosition = localPoint;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
    }

    /// <summary>
    /// Convierte este objeto del Canvas de Screen Space al de World Space.
    /// </summary>
    public void MoveToWorldCanvas()
    {
        // 1️⃣ Guardar posición en pantalla actual
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(
            screenCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : screenCanvas.worldCamera,
            rectTransform.position
        );

        // 2️⃣ Convertir a posición mundial
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            worldCanvas.transform as RectTransform,
            screenPos,
            worldCamera,
            out Vector3 worldPoint
        );

        // 3️⃣ Cambiar el parent al canvas de mundo
        rectTransform.SetParent(worldCanvas.transform, false);
        rectTransform.position = worldPoint;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
    }
}
