using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardBuilder : MonoBehaviour
{
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
    [SerializeField]
    List<Sprite> cardSprites=new List<Sprite>();
    
   public void BuildCard(CardObject cardObj)
    {
        
        

        Image cardSprite = null;
        foreach (Transform child in cardObj.transform)
        {
            cardSprite = child.GetComponentInChildren<Image>();
            if (cardSprite != null)
                break;
        }

        if (cardObj.card != null)
            switch (cardObj.card.cardRarity)//no me juzgueis por esto, soy humano
            {
                case CardRarity.Common:
                    switch (cardObj.card.cardType)
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
                    switch (cardObj.card.cardType)
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
                    switch (cardObj.card.cardType)
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
        Transform cardChild = cardObj.transform.Find("CardTitle");
        if (cardChild != null)
        {
            var comp = cardChild.GetComponent<TextMeshProUGUI>();
            comp.text = cardObj.card.CardName;
        }
        cardChild = cardObj.transform.Find("CardDescription");
        if (cardChild != null)
        {
            var comp = cardChild.GetComponent<TextMeshProUGUI>();
            comp.text = cardObj.card.Description;
        }
        cardChild = cardObj.transform.Find("NCardUses");
        if (cardChild != null)
        {
            var comp = cardChild.GetComponent<TextMeshProUGUI>();
            comp.text = $"{cardObj.card.nUses}";
        }
        cardChild = cardObj.transform.Find("CardIcon");
        if (cardChild != null)
        {
            var comp = cardChild.GetComponent<Image>();
            var sprite = cardSprites.Find(s => s.name == cardObj.card.CardName);
           
            if(sprite!=null)
            {
               comp.enabled = true;
                comp.sprite = sprite;
            }
        }
    }
}
