using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardShop : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private List<GameObject> availableCards;

    [SerializeField] bool canSpawnDrops = true;
    [SerializeField] GameObject HealthDropPrefab;
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
            ShowShop();
            ShootDrops();
        }
    }
    void ShootDrops()
    {
        int nDrops = Random.Range(1, LevelManager.Instance.CurrentNigh);
        float spreadAngle = 30f;
        float shootForce = 15f;
        for(int i = 0; i < nDrops; i++)
        {
            // Crear el objeto
            GameObject prefabDrop = Instantiate(HealthDropPrefab, transform.position, Quaternion.identity);
            Rigidbody rb = prefabDrop.GetComponent<Rigidbody>();

        // Calcular dirección aleatoria dentro del cono de dispersión
        Vector3 baseDir = transform.forward;
        Vector3 randomDir = Quaternion.Euler(
            Random.Range(-spreadAngle, spreadAngle),
            Random.Range(-spreadAngle, spreadAngle),
            0f
        ) * baseDir;

        // Añadir algo de componente vertical hacia arriba
        randomDir = (randomDir + Vector3.up * 0.5f).normalized;

        // Aplicar fuerza impulsiva
        rb.AddForce(randomDir * shootForce, ForceMode.Impulse);
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
