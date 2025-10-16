using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardInventory : MonoBehaviour
{
    //clase que gestiona el acceso a las cartas del jugador
    [SerializeField]
    public Stack<CardObject> attackCards = new Stack<CardObject>();
    [SerializeField]
    public Stack<CardObject> defenseCards = new Stack<CardObject>();
     [SerializeField]
    public Stack<CardObject> utilityCards=new Stack<CardObject>();
    [ContextMenu("Say Cards")]
    public void DebugLogCards()
    {
        Debug.Log($"Tengo {attackCards.Count} cartas de ataque, {defenseCards.Count} cartas de defensa y {utilityCards.Count} de utilidad.");
    }
    public void AddCard(CardObject pCard)
    {
        switch (pCard.card.cardType)
        {
            case CardType.Attack:
                if (!attackCards.Contains(pCard))
                {
                    attackCards.Push(pCard);
                    if (!GetComponent<CardUser>().HasAttackCards)
                    {
                        GetComponent<CardUser>().ReceiveAttackCard(GiveCard(CardType.Attack));
                    }
                }
                break;
            case CardType.Defense:
                if (!defenseCards.Contains(pCard))
                {
                    defenseCards.Push(pCard);
                    if (!GetComponent<CardUser>().HasDefenseCards)
                    {
                        GetComponent<CardUser>().ReceiveDefenseCard(GiveCard(CardType.Defense));
                    }
                }
                break;
            case CardType.Utility:
                if (!utilityCards.Contains(pCard))
                {
                    utilityCards.Push(pCard);
                    if (!GetComponent<CardUser>().HasUtilityCards)
                    {
                        GetComponent<CardUser>().ReceiveUtilityCard(GiveCard(CardType.Utility));
                    }
                }
                break;
        }

    }
    public void AddCards(List<CardObject> pCards)
    {
        foreach(var card in pCards)
        {
            AddCard(card);
        }
    }
    public CardObject GiveCard(CardType cardType)
    {
        //quitar una carta y añadir otra si hay
        switch (cardType)
        {
            case CardType.Attack:
                if(attackCards.Count>0)
                return attackCards.Pop();


                break;
            case CardType.Defense:
                if(defenseCards.Count>0)
                return defenseCards.Pop();

                break;
            case CardType.Utility:
                if(utilityCards.Count>0)
                return utilityCards.Pop();

                break;
        }
        return null;
    }

    
}
