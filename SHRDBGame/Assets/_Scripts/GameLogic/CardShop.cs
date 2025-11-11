using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardShop : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private List<GameObject> availableCards;


    void Start()
    {//buscar al level manager y suscribirse al evento de cambio de noche/dia
     LevelManager.Instance.onNightStateChanged.AddListener(HandleDayNightCycleChanged);
    
        
    }
    public void HandleDayNightCycleChanged(bool isNight)
    {
        if (isNight)
        {
            // Lógica para el ciclo nocturno
            HideShop();
        }
        else
        {
            // Lógica para el ciclo diurno
            ShowShop();            // Aquí puedes agregar el código para cambiar la apariencia o el comportamiento de la tienda durante el día
        }
    }
    void HideShop()
    {
        foreach (GameObject card in availableCards)
        {
            card.SetActive(false);
        }
    }
    void ShowShop()
    {
        foreach (GameObject card in availableCards)
        {
            card.SetActive(true);
            card.GetComponent<ShopCard>().CreateCard();
            card.GetComponent<ShopCard>().ResetItem();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
