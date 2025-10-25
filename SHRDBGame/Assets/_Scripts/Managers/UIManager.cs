using Managers;
using Patterns.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Managers.GameSceneManager;
using static Managers.IManager;
public class UIManager : ASingleton<UIManager>, IManager
{
    //GameManager tiene sus estados pero no le importa en que estado se esta dentro del juego, ahi es donde entra uiManager 
    public enum InGameStates { INGAME, INPAUSE, SELECTINGCARDS, DAYTIME }//ya se que gamemanager tiene inpause y no creo que sea redundante ya que uimanager necesita saber si esta en pausa
    public GameStartMode StartMode => GameStartMode.EARLY;

    [SerializeField] InGameStates inGameStates;
    [Header("Player")]
    [SerializeField]
    GameObject PlayerHUD;


    #region UICards

    [Header("UICards")]
    [SerializeField]
    private GameObject CardPrefab;
    [SerializeField]
    private List<CardObject> UICards;
    [SerializeField]
    private Button ContinueButton;

    //Muy basico se podria hacer de forma dinamica creando y destruyendo cartas
    public void AddCard(CardObject card)
    {
        if (!UICards.Contains(card))
        {
            UICards.Add(card);
        }
    }

    public void BuildCards(List<CardsSO> cards)
    {
        inGameStates = InGameStates.SELECTINGCARDS;

        StartCoroutine(WaitForObject(PlayerHUD));

        for (int i = 0; i < UICards.Count; i++)
        {
            Debug.Log("Construyendo Cartas");
            UICards[i].card = cards[i];
            StartCoroutine(WaitForObject(UICards[i].gameObject));//va demasiado rapido y el objeto a lo mejor no esta activo
            UICards[i].BuildCard();
        }

    }
    IEnumerator WaitForObject(GameObject obj)
    {
        yield return new WaitUntil(() => obj.activeInHierarchy);

        Debug.Log($"{obj.name} ya est� activo!");
    }
    public void LockSelectionCards()
    {
        foreach (var card in UICards)
        {
            card.GetComponent<SelectableUICard>().LockCardSelection();
        }
        //activar Interfaz de continuar
        ContinueButton.gameObject.SetActive(true);
    }
    public void UnLockSelectionCards()
    {
        foreach (var card in UICards)
        {
            card.GetComponent<SelectableUICard>().UnLockCardSelection();
        }
        //desactivar Interfaz de continuar
        ContinueButton.gameObject.SetActive(false);

    }
    public void onEndSelection()
    {
        //
        Debug.Log("Seleccion acabada");
        //bloquear interfaz(desactivar el componente)

        ContinueButton.gameObject.SetActive(false);
        inGameStates = InGameStates.INGAME;
        //mover el resto cartas al inventario, emparentar, ver el orden y ordenar
        foreach (var card in UICards)
        {
            if (!card.GetComponent<SelectableUICard>().isOn)
            {
                switch (card.card.cardType)
                {
                    case CardType.Attack:
                        EmparentCard("CardsDisplay/LeftCard", card.gameObject);
                        break;
                    case CardType.Defense:
                        EmparentCard("CardsDisplay/CenterCard", card.gameObject);
                        break;
                    case CardType.Utility:
                        EmparentCard("CardsDisplay/RightCard", card.gameObject);
                        break;
                }
            }

        }
        foreach (var card in UICards)
        {
            card.GetComponent<SelectableUICard>().interactable = false;
            if (card.GetComponent<SelectableUICard>().isOn)
            {
                switch (card.card.cardType) //primero emparentar los seccionados
                {
                    case CardType.Attack:
                        EmparentCard("CardsDisplay/LeftCard", card.gameObject);
                        break;
                    case CardType.Defense:
                        EmparentCard("CardsDisplay/CenterCard", card.gameObject);
                        break;
                    case CardType.Utility:
                        EmparentCard("CardsDisplay/RightCard", card.gameObject);
                        break;
                }


            }
            card.GetComponent<SelectableUICard>().MoveToCurve(card.transform.parent.position);
            card.GetComponent<SelectableUICard>().Scale(2f);


        }
        //decirle al CardManager que cartas va a usar el jugador, una de cada tipo
        CardManager.Instance.GiveCardToPlayer(FindLastCard("CardsDisplay/LeftCard"));
        CardManager.Instance.GiveCardToPlayer(FindLastCard("CardsDisplay/CenterCard"));
        CardManager.Instance.GiveCardToPlayer(FindLastCard("CardsDisplay/RightCard"));
        //
        CardManager.Instance.GiveCardsToPlayer(UICards);

        //ceder el control del jugador 
        InputManager.Instance.SwitchMapToPlayer();
    }

    void EmparentCard(string objectName, GameObject objectToMove)
    {
        Transform leftCard = PlayerHUD.transform.Find(objectName);

        if (leftCard != null && objectToMove != null)
        {
            objectToMove.transform.SetParent(leftCard, true); // false = adopta la posici�n local del nuevo padre
        }
        else
        {
            Debug.LogWarning($"No se encontr� '{objectName}' o 'objectToMove' no est� asignado");
        }
    }
    CardObject FindLastCard(string objectName)
    {
        Transform parent = PlayerHUD.transform.Find(objectName); // o el que necesites
        if (parent.childCount > 0)
        {
            Transform ultimoHijo = parent.GetChild(parent.childCount - 1);
            Debug.Log("El �ltimo hijo es: " + ultimoHijo.name);
            return ultimoHijo.GetComponent<CardObject>();
        }
        return null;
    }
    #endregion

    #region UIShop
    [Header("UIShop")]
    public GameObject ShopUI;

    public Button ExitShopButton;
    internal void ShowShopText()
    {
        ShopUI.transform.Find("ShopText").gameObject.SetActive(true);
    }
    internal void HideShopText()
    {
        ShopUI.transform.Find("ShopText").gameObject.SetActive(false);
    }
    internal void OpenPanel()
    {
        if (ShopUI != null)
        {
            InputManager.Instance.SwitchMapToUI();
            ShopUI.transform.Find("ShopPanel").gameObject.SetActive(true);
            HideShopText();
        }

    }

    internal void ClosePanel()
    {
        if (ShopUI != null)
        {
            InputManager.Instance.SwitchMapToPlayer();
            ShopUI.transform.Find("ShopPanel").gameObject.SetActive(false);
            ShowShopText();
        }
    }

    #endregion

    #region MainMenu
    [Header("MainMenu")]

    public Button PlayButton;

    void OnPlayPressed()
    {
        GameSceneManager.Instance.LoadSceneById((int)GameSceneManager.SceneIds.GAMESCENE);
    }

    internal void LookForMainMenuCanvas()
    {
        PlayButton = GameObject.Find("CanvasMainMenu/PanelMainMenu/Buttons/PlayButton").GetComponent<Button>();
        GameObject.Find("CanvasMainMenu/PanelMainMenu/Buttons/ExitButton").GetComponent<Button>().onClick.AddListener(QuitApplication);

        //  Debug.Log(PlayButton == null);

        if (PlayButton != null)
        {
            PlayButton.onClick.AddListener(OnPlayPressed);
            //Resetear uiinputmodule por si se ralla
            InputManager.Instance.ResetUIInPutModule(PlayButton.gameObject);
        }
    }
    public void QuitApplication()
    {
        GameManager.Instance.OnEnd();
    }

    #endregion
    #region PauseMenu
    [Header("Pause")]
    [SerializeField]
    public GameObject PauseMenu;
    public void OnPauseUI(bool isPaused)
    {
        if (inGameStates == InGameStates.SELECTINGCARDS)
        {
            GameManager.Instance.BlockPause();
            return;//selectingcards es crucial y bloquea la pausa
        }
        inGameStates = isPaused ? InGameStates.INPAUSE : InGameStates.INGAME;
        PauseMenu.SetActive(isPaused);
        //si en pausa:
        //sacar seleccion de tres
        //enseñar cartas(dejar para mas tarde)
        if (isPaused)
        {
            InputManager.Instance.SwitchMapToUI();
            ShowSelectionCanvas();
        }
        else
        {
            InputManager.Instance.SwitchMapToPlayer();
        }


    }
    public void ShowSelectionCanvas()
    {
        PauseMenu.transform.Find("SelectionCanvas").gameObject.SetActive(true);
        PauseMenu.transform.Find("TabCanvas").gameObject.SetActive(false);
    }
    public void ShowTabCanvas()
    {
        PauseMenu.transform.Find("SelectionCanvas").gameObject.SetActive(false);
        PauseMenu.transform.Find("TabCanvas").gameObject.SetActive(true);
    }
    public void GoBackToMainMenu()
    {
        GameManager.Instance.GoBackToMainMenu();
    }
    #endregion
    #region ManagerLogic
    public void LoadData()
    {
        throw new System.NotImplementedException();
    }

    public void OnEnd()
    {
        Debug.Log($"[{name} cerrando...]");
    }

    public void OnEndGame()
    {
        //TODO eliminar cartas de player hud(o quizas guardarlas para la proxima partida?->otro metodo para guardar preguntar si se quiere guardar partida antes de salir)
        //quitar cartas de player HUD
        foreach (var card in UICards)
        {
            Destroy(card);
        }

        UICards.Clear();
        //tambien eliminar en el playerhud CardsDisplay/LeftCard
        string cardPath = "CardsDisplay/LeftCard";
        foreach (Transform child in PlayerHUD.transform.Find(cardPath))
        {
            Destroy(child.gameObject);
        }
        cardPath = "CardsDisplay/CenterCard";
        foreach (Transform child in PlayerHUD.transform.Find(cardPath))
        {
            Destroy(child.gameObject);
        }
        cardPath = "CardsDisplay/RightCard";
        foreach (Transform child in PlayerHUD.transform.Find(cardPath))
        {
            Destroy(child.gameObject);
        }
       
    }

    public void OnStartGame()
    {
        //1.Al empezar juego se activa la hud del player
        Debug.Log($"[{name}]Empezando juego");
        PauseMenu?.SetActive(false);
        PlayerHUD?.SetActive(true);
        ClosePanel();
        HideShopText();
        //2.UI Cards
        //Ahora queremos instanciar las cartas y manejarlo de forma dinamica para poder tener bien el estado 0 del juego
        for (int i = 0; i < CardManager.Instance.startingCards; i++)
        {
            GameObject uiCard = GameObject.Instantiate(CardPrefab);

            string parent = $"CardsSelector/Card({i + 1})";
            PlayerHUD.transform.Find(parent);
            EmparentCard(parent, uiCard);
            uiCard.GetComponent<RectTransform>().localPosition = Vector3.zero;
            uiCard.GetComponent<RectTransform>().localScale = new Vector3(3, 3);

            //no hay que hacer nada mas porque al crearse y activarse buscaran al uimanager
        }
    }

    public void SaveData()
    {
        throw new System.NotImplementedException();
    }

    public void StartManager()
    {
        GameManager.onPause += OnPauseUI;

        //Pause
        if (PauseMenu != null)
        {
            DontDestroyOnLoad(PauseMenu);
            PauseMenu.SetActive(false);
            //Asignar funciones a los botones del menu de pausa(selectionCanvas)
            PauseMenu.transform.Find("SelectionCanvas/Continue").GetComponent<Button>().onClick.AddListener(GameManager.Instance.UnPauseGame);
            PauseMenu.transform.Find("SelectionCanvas/Settings").GetComponent<Button>().onClick.AddListener(ShowTabCanvas);
            PauseMenu.transform.Find("SelectionCanvas/Quit").GetComponent<Button>().onClick.AddListener(GoBackToMainMenu);

        }
        //Player
        if (PlayerHUD != null)
        {
            DontDestroyOnLoad(PlayerHUD);//quiero que se mantenga la player HUD 
            PlayerHUD.SetActive(false);
            ContinueButton?.onClick.AddListener(onEndSelection);

        }
        //Shop
        if (ShopUI != null)
        {
            DontDestroyOnLoad(ShopUI);//solo va a existir dentro del juego pero quiero mantenerlo tambien por sea caso
            ShopUI.SetActive(true);
            ClosePanel();
            HideShopText();
            ExitShopButton.onClick.AddListener(ClosePanel);
        }
    }




    #endregion
}