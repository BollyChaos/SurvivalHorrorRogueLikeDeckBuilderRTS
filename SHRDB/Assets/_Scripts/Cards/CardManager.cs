using Managers;
using NUnit.Framework;
using Patterns.Singleton;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : ASingleton<CardManager>,IManager
{
    public void LoadData()//sacar los ids de las cartas desbloqueadas
    {
        throw new System.NotImplementedException();
    }

    public void OnEnd()
    {
        throw new System.NotImplementedException();
    }

    public void OnEndGame()
    {
        throw new System.NotImplementedException();
    }

    public void OnStartGame()
    {
        List<CardsSO> cardsReceived = GetComponent<CardShuffler>().ShuffleCards();
        List<CardObject> cardsToGivePlayer=new List<CardObject>();

        for (int i = 0; i < cardsReceived.Count; i++)
        {
            //seria crear el prefab de la carta y build
           GameObject card= new GameObject("Card");
            card.AddComponent<CardObject>();
            card.GetComponent<CardObject>().card = cardsReceived[i];
            cardsToGivePlayer.Add(card.GetComponent<CardObject>());
        }
        FindAnyObjectByType<CardUser>().ReceiveCards(cardsToGivePlayer);
    }

    public void SaveData()
    {
        throw new System.NotImplementedException();
    }

    public void StartManager()
    {
        Debug.Log($"[{name}]:Iniciando...");
    }

    
}
