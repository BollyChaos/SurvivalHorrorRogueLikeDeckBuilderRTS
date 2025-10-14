using System;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableUICard : Toggle
{
    [SerializeField] private Image image;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;


    [Header("Cards Animation")]
    
    [SerializeField] private float offsetY = 40f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float curveAmplitude = 50f;
    [SerializeField] private float wiggleAmount = 50f;
    [SerializeField] private float wiggleSpeed = 0.2f;
    [SerializeField] private float moveDownDuration = 0.3f;
    public LTDescr wiggleTween=null;
    Vector3 originalPos = Vector3.zero;
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
    public void LockCardSelection()
    {
        //si esta on no se puede bloquear
        if(!isOn) lockTogle = true;
        else lockTogle = false;
    }
    public void UnLockCardSelection()
    {
        lockTogle = false;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;
        if (lockTogle) return;//si ya esta hecha la seleccion volver

        base.OnPointerClick(eventData);//de por si la base pone el valor de on

        Debug.Log($"{name} {(isOn ? "Seleccionado" : "Deseleccionado")}");

        EventSystem.current.SetSelectedGameObject(gameObject);

        //si el toggle es on avisar a cardmanager, cuando card manager llegue a las tres cartas necesarias se activa el evento para darselas al jugador
        CardManager.Instance.SelectCards(isOn);

        if (isOn)
        {
            MoveOffsetY(1);

        }
        else
        {
            MoveOffsetY(-1);
        }
    }
    #region CardAnimations
    void MoveOffsetY(int upDown)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        LeanTween.move(rectTransform.gameObject, rectTransform.position + new Vector3(0f, upDown*offsetY, 0f), 0.3f)
                 .setEase(LeanTweenType.easeInOutQuad);
    }
    public void MoveToCurve(Vector3 targetPosition)
    {
        // Posición inicial

        RectTransform rectTransform= GetComponent<RectTransform>();

        Vector3 startPos = rectTransform.position;

        // Creamos puntos intermedios para la curva
        Vector3 midPoint1 = startPos + new Vector3(UnityEngine.Random.Range(-curveAmplitude, curveAmplitude),
                                                   UnityEngine.Random.Range(-curveAmplitude, curveAmplitude), 0f);
        Vector3 midPoint2 = startPos + new Vector3(UnityEngine.Random.Range(-curveAmplitude, curveAmplitude),
                                                   UnityEngine.Random.Range(-curveAmplitude, curveAmplitude), 0f);

        // Creamos el path
        Vector3[] path = new Vector3[] { startPos, midPoint1, midPoint2, targetPosition };

        // Movemos con la curva generada
        LeanTween.move(rectTransform.gameObject, path, duration)
                 .setEase(LeanTweenType.easeInOutSine);
    }
    public void MoveTo(Vector3 targetPosition)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        LeanTween.move(rectTransform.gameObject, targetPosition, duration)
                .setEase(LeanTweenType.easeInOutCubic);
    }
    public void Scale(float scale)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        LeanTween.scale(rectTransform.gameObject, new Vector3(scale,scale), duration)
                 .setEase(LeanTweenType.easeOutBack);
    }
    public void StartIdle()
    {
        // Mueve hacia arriba primero
        RectTransform rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.localPosition;

        MoveOffsetY(2);
        StartWiggle(2);
    }

    private void StartWiggle(float baseY)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        wiggleTween = LeanTween.moveLocalY(rectTransform.gameObject, baseY + wiggleAmount, wiggleSpeed)
                               .setEase(LeanTweenType.easeInOutSine)
                               .setLoopPingPong(-1);
    }

    public void StopIdle()
    {
        // Cancelar wiggle
        if (wiggleTween != null)
        {
            LeanTween.cancel(wiggleTween.id);
            wiggleTween = null;
            MoveToOriginalPos();
            Debug.Log("Cancelando");
        }

        
    }

    private void MoveToOriginalPos()
    {
        MoveTo(transform.parent.position);
    }

    #endregion
}
