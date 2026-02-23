using System.Collections;
using UnityEngine;
using Character.Controls;

public class CardDrop : MonoBehaviour, IInteractable
{

    [SerializeField] private CardObject cardObject;

    [Header("Visuals")]
    [SerializeField] private GameObject cardWorldUI;
    private GameObject instantiatedCardUI;
    [SerializeField] private GameObject cardMesh;
    [SerializeField] private GameObject cardParticles;
    [SerializeField] private GameObject cardBoughtEffect;
    public bool RandomCardOnStart = true;
    public string CardNameOnStart = "Cuchillo";

    private bool isInteractable = false;
    public bool IsInteractable { get => isInteractable; set => isInteractable = value; }
    //si molesta quitar on enable para que esté controlado
    void Start()
    {
        CreateCard();
    }

    [ContextMenu("Crear Carta")]
    public void CreateCard()
    {
        // Crear UI de la carta
        instantiatedCardUI = GameObject.Instantiate(cardWorldUI, transform.Find("CardUIWorldCanvas"));
        cardWorldUI.SetActive(false);
        instantiatedCardUI.transform.localPosition = new Vector3(0.05f, 4.38f, 0.23f);
        instantiatedCardUI.transform.localEulerAngles = new Vector3(75f, 0f, 0f);
        instantiatedCardUI.transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        instantiatedCardUI.GetComponent<PopUp>().SetInitialTransform();

        cardObject = instantiatedCardUI.GetComponent<CardObject>();
        instantiatedCardUI.SetActive(false);
        if (RandomCardOnStart)
            cardObject.card = CardManager.Instance.GiveRandomCard();
        else
            cardObject.card = CardManager.Instance.GiveCardByName(CardNameOnStart);

        if (cardObject.card != null)
            cardObject.BuildCard();
        cardWorldUI.GetComponent<SelectableUICard>().NextCardPhase();

    }


    public void Interact()
    {


        if (isInteractable && cardObject.card != null)
        {
            isInteractable = false;
            Debug.Log($"[{name}] Card Picked!");
            instantiatedCardUI.GetComponent<PopUp>().DeActivateAnimation();
            Destroy(instantiatedCardUI.GetComponent<PopUp>());
            cardParticles.GetComponent<ParticleSystem>().Stop();
            
            UIManager.Instance.PassWorldPosToUI(instantiatedCardUI, instantiatedCardUI.transform.parent.GetComponent<Canvas>());

            cardMesh.GetComponent<IdleFloatAndRotate>().enabled = false;
            cardMesh.SetActive(false);
            cardBoughtEffect.GetComponent<ParticleSystem>().Play();

            if (cardObject != null)
                CardManager.Instance.GiveLateCardToPlayer(cardObject);

            Destroy(gameObject);

        }

    }
    void OnCollisionEnter(Collision collision)
    {
        cardMesh.GetComponent<IdleFloatAndRotate>().ActivateAnimation();
    }

    public void ResetItem()
    {
        cardParticles.SetActive(true);
        cardMesh.SetActive(true);
        instantiatedCardUI.SetActive(false);
    }

    public Transform GetTransform() => this == null ? null : transform;


    public void SetInteractable(bool value)
    {
        isInteractable = value;


        if (instantiatedCardUI != null)
        {
            if (value)
            {
                instantiatedCardUI.SetActive(true);
            }
            else
                instantiatedCardUI.GetComponent<PopUp>().Hide();

            GetComponent<Target>().enabled = !value;
        }
    }

    public string GetInteractionText()
    {

        return $"Presiona E para coger la carta";
    }
}
