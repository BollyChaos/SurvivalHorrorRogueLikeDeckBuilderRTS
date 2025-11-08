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
    public Stack<CardObject> utilityCards = new Stack<CardObject>();
    [ContextMenu("Say N Cards")]
    public void DebugLogCards()
    {
        Debug.Log($"Tengo {attackCards.Count} cartas de ataque, {defenseCards.Count} cartas de defensa y {utilityCards.Count} de utilidad.");
    }
    [ContextMenu("Log All Cards")]
    public void DebugLogAllCards()
    {
        Debug.Log("Cartas de ataque:");
        foreach (var card in attackCards)
        {
            Debug.Log(card.card.CardName);
        }
        Debug.Log("Cartas de defensa:");
        foreach (var card in defenseCards)
        {
            Debug.Log(card.card.CardName);
        }
        Debug.Log("Cartas de utilidad:");
        foreach (var card in utilityCards)
        {
            Debug.Log(card.card.CardName);
        }
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
        foreach (var card in pCards)
        {
            AddCard(card);
        }
    }
    public void AddLateCard(CardObject lCard)
    {
        Stack<CardObject> targetStack = null;

        // 1️⃣ Seleccionar la pila correspondiente según el tipo de carta
        switch (lCard.card.cardType)
        {
            case CardType.Attack:
                targetStack = attackCards;
                break;
            case CardType.Defense:
                targetStack = defenseCards;
                break;
            case CardType.Utility:
                targetStack = utilityCards;
                break;
        }

        if (targetStack == null)
            return;

        // 2️⃣ Evitar duplicados
        if (targetStack.Contains(lCard))
            return;

        // 3️⃣ Usar una pila auxiliar para insertar al fondo
        Stack<CardObject> aux = new Stack<CardObject>();

        while (targetStack.Count > 0)
            aux.Push(targetStack.Pop());

        // Insertar la nueva carta
        targetStack.Push(lCard);

        // Devolver el resto
        while (aux.Count > 0)
            targetStack.Push(aux.Pop());

        // 4️⃣ (Opcional) Si el jugador no tiene carta de ese tipo, darle una
        var cardUser = GetComponent<CardUser>();
        switch (lCard.card.cardType)
        {
            case CardType.Attack:
                if (!cardUser.HasAttackCards)
                    cardUser.ReceiveAttackCard(GiveCard(CardType.Attack));
                break;
            case CardType.Defense:
                if (!cardUser.HasDefenseCards)
                    cardUser.ReceiveDefenseCard(GiveCard(CardType.Defense));
                break;
            case CardType.Utility:
                if (!cardUser.HasUtilityCards)
                    cardUser.ReceiveUtilityCard(GiveCard(CardType.Utility));
                break;
        }
    }

    public CardObject GiveCard(CardType cardType)
    {
        //quitar una carta y añadir otra si hay
        switch (cardType)
        {
            case CardType.Attack:
                if (attackCards.Count > 0)
                    return attackCards.Pop();


                break;
            case CardType.Defense:
                if (defenseCards.Count > 0)
                    return defenseCards.Pop();

                break;
            case CardType.Utility:
                if (utilityCards.Count > 0)
                    return utilityCards.Pop();

                break;
        }
        return null;
    }
    public void OnEndGame()
    {
        foreach (var obj in attackCards)
        {
            Destroy(obj);
        }
        foreach (var obj in defenseCards)
        {
            Destroy(obj);
        }
        foreach (var obj in utilityCards)
        {
            Destroy(obj);
        }
        attackCards.Clear();
        defenseCards.Clear();
        utilityCards.Clear();

    }

}
