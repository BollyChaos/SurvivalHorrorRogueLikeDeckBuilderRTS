using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{

    [SerializeField]
    public
    CardsSO card;
    [Header("Materials")]
    [Header("ComonCards")]
    [SerializeField]
    Material commonCardMatAttack;
    [SerializeField]
    Material commonCardMatDefense;
    [SerializeField]
    Material commonCardMatUtility;
    [Header("RareCards")]
    [SerializeField]
    Material rareCardMatAttack;
    [SerializeField]
    Material rareCardMatDefense;
    [SerializeField]
    Material rareCardMatUtility;
    [Header("SpecialCards")]
    [SerializeField]
    Material specialCardMatAttack;
    [SerializeField]
    Material specialCardMatDefense;
    [SerializeField]
    Material specialCardMatUtility;
    [SerializeField] private int cardNUses;//va a ser la copia del numero de usos de la carta
    public int CardNUses { get => cardNUses; }
    [SerializeField]
    private bool usingCard = false;
    public bool discard = false;
    public bool UsingCard
    {
        get { return usingCard; }
        set { usingCard = value; }
    }

    //Aqui se pueden a�adir materiales distintos para luego las rarezas

    [ContextMenu("BuildCard")]
    public void BuildCard()
    {

        cardNUses = card.nUses;

        // StartCoroutine(BuildCardCoroutine());

        Image cardSprite = null;
        foreach (Transform child in transform)
        {
            cardSprite = child.GetComponentInChildren<Image>();
            if (cardSprite != null)
                break;
        }

        if (card != null)
            switch (card.cardRarity)//no me juzgueis por esto, soy humano
            {
                case CardRarity.Common:
                    switch (card.cardType)
                    {

                        case CardType.Attack:
                            cardSprite.GetComponentInChildren<Image>().material = commonCardMatAttack;

                            break;
                        case CardType.Defense:
                            cardSprite.GetComponentInChildren<Image>().material = commonCardMatDefense;
                            break;
                        case CardType.Utility:
                            cardSprite.GetComponentInChildren<Image>().material = commonCardMatUtility;
                            break;

                    }

                    break;
                case CardRarity.Rare:
                       switch (card.cardType)
                    {

                        case CardType.Attack:
                            cardSprite.GetComponentInChildren<Image>().material = rareCardMatAttack;

                            break;
                        case CardType.Defense:
                            cardSprite.GetComponentInChildren<Image>().material = rareCardMatDefense;
                            break;
                        case CardType.Utility:
                            cardSprite.GetComponentInChildren<Image>().material = rareCardMatUtility;
                            break;

                    }
                    break;
                case CardRarity.Special:
      switch (card.cardType)
                    {

                        case CardType.Attack:
                            cardSprite.GetComponentInChildren<Image>().material = specialCardMatAttack;

                            break;
                        case CardType.Defense:
                            cardSprite.GetComponentInChildren<Image>().material = specialCardMatDefense;
                            break;
                        case CardType.Utility:
                            cardSprite.GetComponentInChildren<Image>().material = specialCardMatUtility;
                            break;

                    }
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
        cardChild = transform.Find("NCardUses");
        if (cardChild != null)
        {
            var comp = cardChild.GetComponent<TextMeshProUGUI>();
            comp.text = $"{card.nUses}";
        }
    }
   
    public void UseCard()
    {
        if (usingCard) return;
        Debug.Log($"Usando la carta:{card.CardName}");
        LevelManager.Instance.AddCardUse();
        usingCard = true;
        //llamar a cardlogichandler y decir su nombre
        --cardNUses;

        var cardChild = transform.Find("NCardUses");
        if (cardChild != null)
        {
            var comp = cardChild.GetComponent<TextMeshProUGUI>();
            comp.text = $"{cardNUses}";
        }
        CardManager.Instance.GetComponent<CardLogicHandler>().UseCard(this);

        if (cardNUses <= 0)
        {
            discard = true;
            try
            {
                Discard();
            }
            catch (System.Exception e)
            {
                Debug.Log($"Error al descartar la carta: {e.Message}");
            }
        }

    }
    public void Discard()
    {
        Debug.Log("Descartando carta");
        //gameObject.SetActive(false);
        if (gameObject != null)
            Destroy(gameObject);
    }
}
