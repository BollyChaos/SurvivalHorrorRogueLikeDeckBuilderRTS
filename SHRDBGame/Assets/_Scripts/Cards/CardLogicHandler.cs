using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CardLogicHandler : MonoBehaviour
{
    //TODO: USAR PREFABS Y NO OBJETOS DE ESCENA, IMPORTANTISIMO(USAR OBJECT POOL TAMBIEN)
    //Usar un diccionario donde hay un string nombre de la carta, y una referencia a la funcion que hace, ha de ser puesto a mano
    //Un diccionario no sale en el inspector
    //Dictionary<string,>
    [SerializeField]
    List<GameObject> cardEffects = new List<GameObject>();
    Dictionary<string, GameObject> cardEffectsInScene = new Dictionary<string, GameObject>();
    [Header("Debug")]
    [SerializeField] bool debug = true;
    [ShowIf("debug")]
    [SerializeField] string cardToUseAlways = "Curacion";
    public void SetPlayerTransform(Transform playerTransform)
    {
        foreach (GameObject cardEffect in cardEffects)
        {
            ICardAction cardAction = cardEffect.GetComponent<ICardAction>();
            if (cardAction != null)
            {
                cardAction.PlayerTransform = playerTransform;
            }
        }
    }
    internal void UseCard(CardObject cardObj)
    {
        //buscar la carta en el diccionario, si no est� usar default
        //gestionar l�gica de usar una carta(duraci�n, si es la misma no interrumpir, el manejo de los usos de la carta, descartarla
        Debug.Log($"[CardLogicHandler]Me ha llegado la carta {cardObj.card.CardName}");
        if (debug)
        {
            //buscar el prefab asset
            GameObject foundCardEffectDebug = cardEffects.Find(n => n.name == cardToUseAlways);

            if (foundCardEffectDebug != null)
            {
                ExecuteCard(foundCardEffectDebug, cardObj);
                return;
            }
            else
            {
                Debug.LogError("No existe ese nombre, comprueba la lista o que esté bien escrito");
            }
        }
        GameObject foundCardEffect = cardEffects.Find(n => n.name == cardObj.card.CardName);
        if (foundCardEffect != null)
        {
            ExecuteCard(foundCardEffect, cardObj);
            return;
        }
        else
        {
            //cardEffects[cardObj.card.CardName].Invoke(cardObj);
            Debug.Log("[CardLogicHandler]No he encontrado la carta, uso default");
            DefaultCardBehaviour(cardObj);
        }

    }
    void ExecuteCard(GameObject foundCardEffect, CardObject cardObj)
    {
        //si existe buscar en el diccionario(por nombre)
        GameObject cardEffect = null;
        if (!cardEffectsInScene.TryGetValue(foundCardEffect.name, out cardEffect))
        {
            cardEffect = Instantiate(foundCardEffect, transform);
            cardEffect.name = foundCardEffect.name;
            cardEffectsInScene[cardEffect.name] = cardEffect;
        }
        cardEffect.GetComponent<ICardAction>().PlayerTransform = FindAnyObjectByType<SimplePlayerController>().transform;
        cardEffect.GetComponent<ICardAction>().ExecuteCardAction(cardObj);

        // foundCardEffectDebug.GetComponent<ICardAction>().PlayerTransform=FindAnyObjectByType<SimplePlayerController>().transform;
        // foundCardEffectDebug.GetComponent<ICardAction>().ExecuteCardAction(cardObj);

        return;
    }
    public void DefaultCardBehaviour(CardObject cardObj)
    {
        Debug.Log("[CardLogicHandler]Usando comportamiento por defecto de carta");
        cardObj.UsingCard = false;
    }



}
