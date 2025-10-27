using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLogicHandler : MonoBehaviour
{
    //Usar un diccionario donde hay un string nombre de la carta, y una referencia a la funcion que hace, ha de ser puesto a mano
    //Un diccionario no sale en el inspector
    //Dictionary<string,>
    Dictionary<string, Action<CardObject>> cardEffects=new Dictionary<string, Action<CardObject>>();
    void Awake()
    {
        //se añade el nombre de la carta y el comportamiento(la funcion que dispara la corrutina)
        cardEffects.Add("hola", DefaultCardBehaviourFunction);
    }
    internal void UseCard(CardObject cardObj)
    {
        //buscar la carta en el diccionario, si no est� usar default
        //gestionar l�gica de usar una carta(duraci�n, si es la misma no interrumpir, el manejo de los usos de la carta, descartarla
        Debug.Log($"[CardLogicHandler]Me ha llegado la carta {cardObj.card.CardName}");
        if (cardEffects.ContainsKey(cardObj.card.CardName))
        {
            cardEffects[cardObj.card.CardName].Invoke(cardObj);
        }
        
    }
    IEnumerator DefaultCardBehaviour(CardObject cardObj)
    {
        
        yield return new WaitForSeconds(3f);

        cardObj.UsingCard = false;
        //GameObject.Find("Player").GetComponent<PlayerStats>()
    }
    void DefaultCardBehaviourFunction(CardObject cardObject)
    {
        StartCoroutine(DefaultCardBehaviour(cardObject));
        
    }

    
}
