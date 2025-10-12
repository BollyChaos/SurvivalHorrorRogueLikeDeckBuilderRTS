using Managers;
using Patterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : ASingleton<CardManager>,IManager
{
    int playerCards = 3;
    int playerCardsCounter= 0;  
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
        Debug.Log($"[{name}]:Empezando juego");
        playerCardsCounter = 0;

        List<CardsSO> cardsReceived = GetComponent<CardShuffler>().ShuffleCards();

        UIManager.Instance?.BuildCards(cardsReceived);



        //for (int i = 1; i < cardsReceived.Count; i++)
        //{
        //    //Ahora se dan a la interfaz y que las construya

        //    CardsSO card= cardsReceived[i];
        //    CardIterator.MoveNext(card);

        //}
        ////FindAnyObjectByType<CardUser>().ReceiveCards(cardsToGivePlayer);
    }
    public void SelectCards(bool selected)
    {
        //si se ha seleccionado sumar si no restar
        playerCardsCounter += selected ? 1: -1;
        if(playerCardsCounter >= playerCards)
        {
            Debug.Log("Cartas seleccionadas");
            //bloquear la seleccion de mas cartas
            //activar algun boton de continuar
            //ordenar cartas
        }
    }
    public void GiveCardsToPlayer()
    {

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
