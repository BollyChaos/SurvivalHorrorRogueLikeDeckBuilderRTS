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
    List<CardsSO> allCardsFromFolder = new List<CardsSO>();

    [SerializeField]
    List<CardsSO> unlockedCardsAvailable = new List<CardsSO>();

    [SerializeField]
    List<CardsSO> givenCardsToPlayer = new List<CardsSO>();

    [SerializeField]
    List<float> cardTypeWeight = new List<float>();
    [SerializeField]
    float cardTypeDecayFactor;

    [SerializeField]
    List<float> cardRaritiesWeights = new List<float>();
    [SerializeField]
    List<float> cardRaritiesDecayFactor = new List<float>();
    [SerializeField]
    public int nCardsToGive = 8;
    private bool resetPool;

    public void Awake()
    {
        allCardsFromFolder = new List<CardsSO>(Resources.LoadAll<CardsSO>(cardsFolderName));
        foreach (var card in allCardsFromFolder)
        {
            if(card.cardId!=-1)
            card.cardId = CardHasher.GetUniqueID();

        }
    }
    public void Start()
    {
        foreach (var card in allCardsFromFolder)
        {
            if (card.cardId != -1)//la gracia esta en que haya una carta empty, entonces esa no necesitamos un id unico
                if (card.unlocked)
                {
                    unlockedCardsAvailable.Add(card);//si la carta esta desbloqueada a�adir
                }
        }
    }

    public List<CardsSO> ShuffleCards()
    {
        List<CardsSO> cardsToReturn = new List<CardsSO>();

        //Primero averiguar el tipo de cartas que van a salir
        //como los tipos de cartas son equiprobables poner siempre el mismo peso
        for (int i = 0; i < cardTypeWeight.Count; i++)
        {
            cardTypeWeight[i] = 1;
        }
        DynamicProbability.SetWeights(cardTypeWeight.ToArray(), cardTypeDecayFactor);
        int[] cardTypes = DynamicProbability.RollNTimes(nCardsToGive);


        //Luego averiguar la rareza de las cartas por tipo
        DynamicProbability.SetWeights(cardRaritiesWeights.ToArray(), cardRaritiesDecayFactor.ToArray());
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
        List<CardsSO> availableCards = unlockedCardsAvailable.Except(givenCardsToPlayer).ToList();
        //Casos posibles:
        //1.Encontrar el tipo y la rareza
        //2.Encontrar el tipo pero no la rareza->dar otra rareza
        //3.No encontrar el tipo y si la rareza-> dar otr o tipo
        //4.No encontrar nada(no hay mas cartas o no hay de esa clase)-> volver a llenar la pool
        for (int i = 0; i < nCardsToGive; i++)
        {
            var sameTypeSameRarity = availableCards//mismo tipo y rareza
                .Where(c => (int)c.cardType == cardTypes[i] && (int)c.cardRarity == cardRarities[i] && c.cardId != -1)
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
                chosen = sameTypeSameRarity[UnityEngine.Random.Range(0,sameTypeSameRarity.Count)]; // Caso 1}
            }
            else if (sameType.Count > 0)
            {
                chosen = sameType[UnityEngine.Random.Range(0,sameType.Count)]; // Caso 2
            }
            else if (sameRarity.Count > 0)
            {
                chosen = sameRarity[UnityEngine.Random.Range(0,sameRarity.Count)]; // Caso 3
            }
            else
            {
                availableCards = unlockedCardsAvailable.Except(cardsToReturn).ToList();
                resetPool = true;

                if (availableCards.Count > 0)
                    chosen = availableCards[0]; // Caso 4
            }

            if (chosen != null)
            {
                givenCardsToPlayer.Add(chosen);
                cardsToReturn.Add(chosen);
                availableCards.Remove(chosen);
            }
        }
        return cardsToReturn;
    }

    public void ResetPool()
    {
        givenCardsToPlayer=null;
        unlockedCardsAvailable = null;
        givenCardsToPlayer = new List<CardsSO>();
        unlockedCardsAvailable = new List<CardsSO>();
        foreach (var card in allCardsFromFolder)
        {
            if (card.unlocked)
            {
                unlockedCardsAvailable.Add(card);//si la carta esta desbloqueada a�adir
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
    public CardsSO FindCardById(int Id)
    {
        return unlockedCardsAvailable.Find( c=>c.cardId == Id);
    }
    public void OnDestroy()
    {
        //Limpiar las listas porque unity se ralla
        allCardsFromFolder = null;
        unlockedCardsAvailable = null;
        givenCardsToPlayer = null;
    }
}
