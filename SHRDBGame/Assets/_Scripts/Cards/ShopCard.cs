using System.Collections;
using UnityEngine;
using Character.Controls;

public class ShopCard : MonoBehaviour, IInteractable
{
    [Header("Shop Logic")]
    [SerializeField] private int price = 1;
    [SerializeField] private CardObject cardObject;

    [Header("Visuals")]
    [SerializeField] private GameObject cardWorldUI;
    private GameObject instantiatedCardUI;
    [SerializeField] private GameObject cardMesh;
    [SerializeField] private GameObject cardParticles;
    [SerializeField] private GameObject cardBoughtEffect;
    private bool lockItem = false;
    private bool isInteractable = false;
    public bool IsInteractable { get { return isInteractable; } set { isInteractable = value; } }

    [Header("Audio")]
    [SerializeField] private ASoundPlayer shopSoundPlayer;
    [SerializeField] private int buySoundIndex = 0;


    public void CreateCard()
    {
        instantiatedCardUI = GameObject.Instantiate(cardWorldUI, transform.Find("CardUIWorldCanvas"));
        cardWorldUI.SetActive(false);

        instantiatedCardUI.transform.localPosition = new Vector3(0.049987793f, 8.38000488f, 0.230000004f);
        instantiatedCardUI.transform.localEulerAngles = new Vector3(75f, 0f, 0f);
        instantiatedCardUI.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        instantiatedCardUI.GetComponent<PopUp>().SetInitialTransform();

        cardObject = instantiatedCardUI.GetComponent<CardObject>();
        instantiatedCardUI?.SetActive(false);

        cardObject.card = CardManager.Instance.GiveRandomCard();
        if (cardObject.card != null)
        {
            cardObject.BuildCard();
        }

        if (cardObject.card != null)
            price = (int)cardObject.card.cardRarity + 1;
    }

    public int GetPrice()
    {
        return price;
    }

    public void Interact()
    {
        if (lockItem) return;

        if (FindAnyObjectByType<Economy>().SpendCoins(price))
        {
            if (shopSoundPlayer != null)
                shopSoundPlayer.PlaySound(buySoundIndex);

            Debug.Log($"[{name}] Card Bought!");
            lockItem = true;

            cardParticles.GetComponent<ParticleSystem>().Stop();

            UIManager.Instance.PassWorldPosToUI(instantiatedCardUI, instantiatedCardUI.transform.parent.GetComponent<Canvas>());

            cardMesh.GetComponent<IdleFloatAndRotate>().enabled = false;
            cardMesh.SetActive(false);
            cardBoughtEffect.GetComponent<ParticleSystem>().Play();

            if (cardObject != null)
                CardManager.Instance.GiveLateCardToPlayer(cardObject);
        }
        else
        {
            Debug.Log($"[{name}] Not enough coins to buy the card!");
        }
    }

    public void ResetItem()
    {
        lockItem = false;
        cardParticles.SetActive(true);
        cardMesh.SetActive(true);
        instantiatedCardUI.SetActive(false);
    }

    public Transform GetTransform()
    {
        return this.transform;
    }

    public void SetInteractable(bool value)
    {
        isInteractable = value;
        if (lockItem) return;

        if (instantiatedCardUI != null)
        {
            if (value)
            {
                instantiatedCardUI.SetActive(true);
            }
            else
            {
                instantiatedCardUI.GetComponent<PopUp>().Hide();
            }
        }
    }

    public string GetInteractionText()
    {
        if (lockItem) return string.Empty;

        string inttext = $"Presiona E para comprar la carta por {price}";
        Debug.Log(inttext);

        return inttext;
    }
}