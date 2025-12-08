using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardObject : MonoBehaviour
{

    [Header("Card Data")]
    [SerializeField]
    public
    CardsSO card;
    [SerializeField] private int cardNUses;//va a ser la copia del numero de usos de la carta
    public int CardNUses { get => cardNUses; }
    [SerializeField]
    private bool usingCard = false;
    public bool discard = false;
    public bool UsingCard
    {
        get { return usingCard; }
        set { usingCard = value; }
    }

    //Aqui se pueden a�adir materiales distintos para luego las rarezas

    public void BuildCard()
    {

        cardNUses = card.nUses;

        //Ahora esta clase construye las cartas
        CardManager.Instance.GetComponent<CardBuilder>().BuildCard(this);
        
    }

    public void UseCard()
    {
        if (usingCard) return;
        Debug.Log($"Usando la carta:{card.CardName}");
        LevelManager.Instance.AddCardUse();
        usingCard = true;
        //llamar a cardlogichandler y decir su nombre
        --cardNUses;

        var cardChild = transform.Find("NCardUses");
        if (cardChild != null)
        {
            var comp = cardChild.GetComponent<TextMeshProUGUI>();
            comp.text = $"{cardNUses}";
        }
        CardManager.Instance.GetComponent<CardLogicHandler>().UseCard(this);

        if (cardNUses <= 0)
        {
            discard = true;
            try
            {
                Discard();
            }
            catch (System.Exception e)
            {
                Debug.Log($"Error al descartar la carta: {e.Message}");
            }
        }

    }
    public void Discard()
    {
        Debug.Log("Descartando carta");
        //gameObject.SetActive(false);
        if (gameObject != null)
            Destroy(gameObject);
    }
}
