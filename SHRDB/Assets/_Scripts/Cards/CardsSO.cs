using UnityEngine;
public enum CardType { Attack, Defense, Utility }
public enum CardRarity { Common, Rare, Special }

[CreateAssetMenu(fileName = "CardsSO", menuName = "ScriptableObjects/CardsSO")]
public class CardsSO : ScriptableObject
{
    [SerializeField]
    public int cardId;//(o usar enum)
    
    [SerializeField]
    public string CardName;
    
    [SerializeField]
    public string Description;

    [SerializeField]
    public int nUses = 1;
    [SerializeField]
    public bool unlocked = true;

    [SerializeField]
    public CardType cardType;
    [SerializeField] 
    public CardRarity cardRarity;

}
