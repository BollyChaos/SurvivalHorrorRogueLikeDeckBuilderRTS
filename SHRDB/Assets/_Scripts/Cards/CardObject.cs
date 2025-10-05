using UnityEngine;

public class CardObject : MonoBehaviour
{
    [SerializeField]
    public
    CardsSO card;
    private void BuildCard()
    {
        //basado en las propiedades de la carta en un prefab vacio mete los valores del so
    }
   public void UseCard()
    {
        Debug.Log($"Usando la carta:{card.CardName}");
    }
   private void Discard()
    {

    }
}
