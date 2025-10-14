using UnityEngine;
public enum CardType { Attack, Defense, Utility }
public enum CardRarity { Common, Rare, Special }

[CreateAssetMenu(fileName = "CardsSO", menuName = "ScriptableObjects/CardsSO")]
public class CardsSO : ScriptableObject
{
    [SerializeField]
    public int cardId;//(o usar enum)
    
    [SerializeField]
    private string cardName;
    public string CardName { get { return cardName; } }//get necesario para evitar que se corrompa la cache
    
    [SerializeField]
    private string description;

    public string Description { get { return description; } }//get necesario por el mismo motivo

    [SerializeField]
    public int nUses = 1;
    [SerializeField]
    public bool unlocked = true;

    [SerializeField]
    public CardType cardType;
    [SerializeField] 
    public CardRarity cardRarity;

}
