using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardShuffler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    string cardsFolderName = "CardsInformation";
    [SerializeField]
    List<CardsSO> allCards;

    [SerializeField]
    List<CardsSO> unlockedCards;

    [SerializeField]
    List<CardsSO> givenCards;

    [SerializeField]
    float[] cardTypesWeights;
    [SerializeField]
    float cardTypesDecayFactor;

    [SerializeField]
    float[] cardRarityWeights;
    [SerializeField]
    float[] cardRarityDecayFactor;
    [SerializeField]
    int nCardsToGive = 10;
    private bool resetPool;
    public void Start()
    {
        allCards = new List<CardsSO>(Resources.LoadAll<CardsSO>(cardsFolderName));
        foreach (var card in allCards)
        {
            if (card.unlocked)
            {
                unlockedCards.Add(card);//si la carta esta desbloqueada añadir
            }
        }
    }

    [ContextMenu("ShuffleCards")]
    public List<CardsSO> ShuffleCards()
    {
        List<CardsSO> cardsToReturn = new List<CardsSO>();

        //Primero averiguar el tipo de cartas que van a salir
        //como los tipos de cartas son equiprobables poner siempre el mismo peso
        for (int i = 0; i < cardTypesWeights.Length; i++)
        {
            cardTypesWeights[i] = 1;
        }
        DynamicProbability.SetWeights(cardTypesWeights, cardTypesDecayFactor);
        int[] cardTypes = DynamicProbability.RollNTimes(nCardsToGive);


        //Luego averiguar la rareza de las cartas por tipo
        DynamicProbability.SetWeights(cardRarityWeights, cardRarityDecayFactor);
        int[] cardRarities = DynamicProbability.RollNTimesNEP(nCardsToGive);

        for (int i = 0; i < nCardsToGive; i++)
        {
            PrintCard(cardTypes[i], cardRarities[i]);
        }
        cardsToReturn = FindCards(cardTypes, cardRarities);

        return cardsToReturn;
    }

    private List<CardsSO> FindCards(int[] cardTypes, int[] cardRarities)
    {
        if (resetPool) ResetPool();
        List<CardsSO> cardsToReturn = new List<CardsSO>();
        //Primero es restar los conjuntos de cartas desbloqueadas y las cartas dadas, asi tenemos las disponibles
        List<CardsSO> availableCards = unlockedCards.Except(givenCards).ToList();
        //Casos posibles:
        //1.Encontrar el tipo y la rareza
        //2.Encontrar el tipo pero no la rareza->dar otra rareza
        //3.No encontrar el tipo y si la rareza-> dar otro tipo
        //4.No encontrar nada(no hay mas cartas o no hay de esa clase)-> volver a llenar la pool
        for (int i = 0; i < nCardsToGive; i++)
        {
            var sameTypeSameRarity = availableCards//mismo tipo y rareza
                .Where(c => (int)c.cardType == cardTypes[i] && (int)c.cardRarity == cardRarities[i]&&c.cardId!=-1)
                .ToList();

            var sameType = availableCards//solo mismo tipo
                .Where(c => (int)c.cardType == cardTypes[i] && c.cardId != -1)
                .ToList();

            var sameRarity = availableCards//solo misma rareza
                .Where(c => (int)c.cardRarity == cardRarities[i] && c.cardId != -1)
                .ToList();

            CardsSO chosen = null;

            if (sameTypeSameRarity.Count > 0)
            {
                chosen = sameTypeSameRarity[0]; // Caso 1}
            }
            else if (sameType.Count > 0)
            {
                chosen = sameType[0]; // Caso 2
            }
            else if (sameRarity.Count > 0)
            {
                chosen = sameRarity[0]; // Caso 3
            }
            else
            {
                availableCards = unlockedCards.Except(cardsToReturn).ToList();
                resetPool = true;
                
                if (availableCards.Count > 0)
                    chosen = availableCards[0]; // Caso 4
            }

            if (chosen != null)
            {
                givenCards.Add(chosen);
                cardsToReturn.Add(chosen);
                availableCards.Remove(chosen);
            }
        }
        return cardsToReturn;
    }

    private void ResetPool()
    {
        givenCards.Clear();
        unlockedCards.Clear();
        foreach (var card in allCards)
        {
            if (card.unlocked)
            {
                unlockedCards.Add(card);//si la carta esta desbloqueada añadir
            }
        }
        resetPool = false;
    }

    void PrintCard(int CrdType, int CrdRarity)
    {
        string cardType = string.Empty;
        string cardRarity = string.Empty;
        switch (CrdType)
        {
            case (int)CardType.Attack:
                cardType = "Attack";
                break;
            case (int)CardType.Defense:
                cardType = "Defense";
                break;
            case (int)CardType.Utility:
                cardType = "Utility";
                break;
        }
        switch (CrdRarity)
        {
            case (int)CardRarity.Common:
                cardRarity = "Common";
                break;
            case (int)CardRarity.Rare:
                cardRarity = "Rare";
                break;
            case (int)CardRarity.Special:
                cardRarity = "Special";
                break;
        }
        Debug.Log($"Carta de tipo: {cardType}. Rareza:{cardRarity}");
    }
}
