using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{

    [SerializeField]
    public
    CardsSO card;
    [SerializeField]
    Color AttackColor;
    [SerializeField]
    Color DefenseColor;
    [SerializeField]
    Color UtilityColor;
    //Aqui se pueden añadir materiales distintos para lueog las rarezas

    [ContextMenu("BuildCard")]
    public void BuildCard()
    {


        // StartCoroutine(BuildCardCoroutine());

        Image cardSprite = null;
        foreach (Transform child in transform)
        {
            cardSprite = child.GetComponentInChildren<Image>();
            if (cardSprite != null)
                break;
        }

        if (card != null)
            switch (card.cardType)
            {
                case CardType.Attack:
                    cardSprite.color = AttackColor;
                    break;
                case CardType.Defense:
                    cardSprite.color = DefenseColor;
                    break;
                case CardType.Utility:
                    cardSprite.color = UtilityColor;
                    break;
            }
        //Los hijos cardtitle y carddesccription contienen el titulo y la descripcion respectivamente
        Transform cardChild = transform.Find("CardTitle");
        if (cardChild != null)
        {
            var comp = cardChild.GetComponent<TextMeshProUGUI>();
            comp.text = card.CardName;
        }
        cardChild = transform.Find("CardDescription");
        if (cardChild != null)
        {
            var comp = cardChild.GetComponent<TextMeshProUGUI>();
            comp.text = card.Description;
        }
    }
    private IEnumerator BuildCardCoroutine()
    {
        //basado en las propiedades de la carta en un prefab vacio mete los valores del so
        //1. Ver tipo 
        Image cardSprite = null;
        foreach (Transform child in transform)
        {
            cardSprite = child.GetComponentInChildren<Image>();
            if (cardSprite != null)
                break;
        }

        yield return null;
        if (card != null)
            switch (card.cardType)
            {
                case CardType.Attack:
                    cardSprite.color = AttackColor;
                    break;
                case CardType.Defense:
                    cardSprite.color = DefenseColor;
                    break;
                case CardType.Utility:
                    cardSprite.color = UtilityColor;
                    break;
            }
        yield return null;
        //Los hijos cardtitle y carddesccription contienen el titulo y la descripcion respectivamente
        Transform cardChild = transform.Find("CardTitle");
        if (cardChild != null)
        {
            var comp = cardChild.GetComponent<TextMeshProUGUI>();
            comp.text = card.CardName;
        }
        cardChild = transform.Find("CardDescription");
        if (cardChild != null)
        {
            var comp = cardChild.GetComponent<TextMeshProUGUI>();
            comp.text = card.Description;
        }
        yield return null;
    }
    public void UseCard()
    {
        Debug.Log($"Usando la carta:{card.CardName}");
        //llamar a cardlogichandler y decir su nombre
        CardManager.Instance.GetComponent<CardLogicHandler>().UseCard(card.CardName);
    }
    private void Discard()
    {

    }
}
