using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectableUICard : Button
{
    [SerializeField] private Image image;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;

    private bool isSelected = false;

    void Awake()
    {
        if (!image) image = GetComponent<Image>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        image.color = selectedColor;
        Debug.Log($"Seleccionado: {name}");
    }

    // Cuando el objeto se deselecciona
    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;
        image.color = normalColor;
        Debug.Log($"Deseleccionado: {name}");
    }

    // Cuando se hace clic con el ratón o toque táctil
    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
