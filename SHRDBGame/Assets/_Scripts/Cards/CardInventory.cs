using System.Collections.Generic;
using UnityEngine;

public class CardInventory : MonoBehaviour
{
    //clase que gestiona el acceso a las cartas del jugador
    [SerializeField]
    public List<CardsSO> playerCards;
    public void AddCard(CardsSO pCard)
    {
        playerCards.Add(pCard);
    }
    public void RemoveCard(CardsSO pCard)
    {
        playerCards.Remove(pCard);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
