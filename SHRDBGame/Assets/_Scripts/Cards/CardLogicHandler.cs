using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLogicHandler : MonoBehaviour
{

    //Usar un diccionario donde hay un string nombre de la carta, y una referencia a la funcion que hace, ha de ser puesto a mano
    //Un diccionario no sale en el inspector
    //Dictionary<string,>
[SerializeField]    List<GameObject> cardsEffects = new List<GameObject>();
   
    internal void UseCard(CardObject cardObj)
    {
        //buscar la carta en el diccionario, si no est� usar default
        //gestionar l�gica de usar una carta(duraci�n, si es la misma no interrumpir, el manejo de los usos de la carta, descartarla
        Debug.Log($"[CardLogicHandler]Me ha llegado la carta {cardObj.card.CardName}");
        GameObject cardToUse = cardsEffects.Find(ce => name == cardObj.card.CardName);//busca el prefab de la carta
        if (cardToUse!=null)//si lo encuentra le va a decir que haga su efecto de carta
        {
            //instanciar, usar y destruir(ellos mismos al acabar)
            cardToUse.GetComponent<ICardEffect>().UseCard();
            cardObj.UsingCard = false;
        }
       
        
    }
    

    
}
