using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardShop : MonoBehaviour
{
    [SerializeField] private List<GameObject> availableCards;

    [SerializeField] private bool canSpawnDrops = true;
    [SerializeField] private GameObject HealthDropPrefab;

    void Start()
    {
        LevelManager.Instance.onNightStateChanged.AddListener(HandleDayNightCycleChanged);
    }

    public void HandleDayNightCycleChanged(bool isNight)
    {
        if (isNight)
        {
            HideShop();
        }
        else
        {
            ShowShop();
            if(canSpawnDrops)
            ShootDrops();
        }
    }

    void ShootDrops()
    {
        int nDrops = Random.Range(1, LevelManager.Instance.CurrentNigh);
        float spreadAngle = 30f;
        float shootForce = 15f;

        for (int i = 0; i < nDrops; i++)
        {
            GameObject prefabDrop = Instantiate(HealthDropPrefab, transform.position, Quaternion.identity);
            Rigidbody rb = prefabDrop.GetComponent<Rigidbody>();

            Vector3 baseDir = transform.forward;
            Vector3 randomDir = Quaternion.Euler(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0f
            ) * baseDir;

            randomDir = (randomDir + Vector3.up * 0.5f).normalized;
            rb.AddForce(randomDir * shootForce, ForceMode.Impulse);
        }
    }

    void HideShop()
    {
        foreach (GameObject card in availableCards)
            card.SetActive(false);
    }

    void ShowShop()
    {
        foreach (GameObject card in availableCards)
        {
            card.SetActive(true);
            ShopCard shopCard = card.GetComponent<ShopCard>();
            shopCard.CreateCard();
            shopCard.ResetItem();
        }
    }

    void Update() { }
}