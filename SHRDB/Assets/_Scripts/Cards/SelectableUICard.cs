using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableUICard : Toggle
{
    [SerializeField] private Image image;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;

    public bool lockTogle = false;
    void Awake()
    {
        base.Awake();
        if (image==null) image = GetComponent<Image>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        LookForUIManager();

    }
    private void LookForUIManager()
    {

        UIManager manager=UIManager.Instance;
        if (manager != null)
        {
            var card = GetComponent<CardObject>();

            manager.AddCard(card);
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        image.color = isOn ? selectedColor : normalColor;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        Debug.Log($"{name} seleccionado (navegación)");
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        Debug.Log($"{name} deseleccionado (navegación)");
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (lockTogle) return;//si ya esta hecha la seleccion volver

        base.OnPointerClick(eventData);
        Debug.Log($"{name} {(isOn ? "Seleccionado" : "Deseleccionado")}");

        EventSystem.current.SetSelectedGameObject(gameObject);

        //si el toggle es on avisar a cardmanager, cuando card manager llegue a las tres cartas necesarias se activa el evento para darselas al jugador
        CardManager.Instance.SelectCards(isOn);
        if (isOn)
        {
            // Calcula la nueva posición sumando offsetY
            
        }
    }
}
