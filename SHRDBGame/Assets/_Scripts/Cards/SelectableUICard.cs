using System;
using System.IO;
using System.Xml.Linq;
using Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableUICard : Toggle
{
    //TODO: COMPATIBLE CON MANDO
    public enum CardPhase { SELECTION, INGAME }
    [SerializeField]
    private CardPhase cardPhase = CardPhase.SELECTION;
    [SerializeField] private  Image uicardImage;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;


    [Header("Cards Animation")]

    [SerializeField] private float offsetY = 40f;
   
    public bool lockTogle = false;
   protected override void Awake()
    {
        base.Awake();
        if (uicardImage == null) uicardImage = GetComponent<Image>();
    }
    protected override void OnEnable()
    {
        if (!interactable) return;
        base.OnEnable();
        LookForUIManager();

    }
    private void LookForUIManager()
    {

        UIManager manager = UIManager.Instance;
        if (manager != null)
        {
            var card = GetComponent<CardObject>();

            manager.AddCard(card);
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        uicardImage.color = isOn ? selectedColor : normalColor;
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
        if (!isOn) lockTogle = true;
        else lockTogle = false;
    }
    public void UnLockCardSelection()
    {
        lockTogle = false;
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
       if (eventData.button != PointerEventData.InputButton.Left)
        return;
        if (!interactable) return;
        switch (cardPhase)
        {

            case CardPhase.SELECTION:
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
                break;
            case CardPhase.INGAME:
                //Debug.Log(name + " carta seleccionada");
                FindObjectOfType<CardUser>().SetCardType = (GetComponent<CardObject>().card.cardType);
                FindObjectOfType<CardUser>().ReadInputCardMobile();
                break;
        }
    }
    public void NextCardPhase()
    {
        cardPhase = CardPhase.INGAME;
        if (GameManager.Instance.gamePlatform != GamePlatform.WebGL_Mobile)
        {
            interactable = false;

        }
        isOn = false;
    }
    #region CardAnimations
    void MoveOffsetY(int upDown)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        LeanTween.move(rectTransform.gameObject, rectTransform.position + new Vector3(0f, upDown * offsetY, 0f), 0.3f)
                 .setEase(LeanTweenType.easeInOutQuad);
    }
   
    #endregion
}
