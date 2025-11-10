using Character.Settings;
using Managers;
using Patterns.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Managers.GameSceneManager;
using static Managers.IManager;
public class UIManager : ASingleton<UIManager>, IManager
{
    //GameManager tiene sus estados pero no le importa en que estado se esta dentro del juego, ahi es donde entra uiManager 
    public enum InGameStates { INGAME, INDIALOG, INPAUSE, SELECTINGCARDS, DAYTIME }//ya se que gamemanager tiene inpause y no creo que sea redundante ya que uimanager necesita saber si esta en pausa
    public GameStartMode StartMode => GameStartMode.EARLY;
    [SerializeField] InGameStates previousInGameState;
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
        InputManager.Instance.SwitchMapToUI();
        previousInGameState = inGameStates;
        inGameStates = InGameStates.SELECTINGCARDS;

        StartCoroutine(WaitForObject(PlayerHUD));

        for (int i = 0; i < UICards.Count; i++)
        {
            Debug.Log("Construyendo Cartas");
            UICards[i].gameObject.SetActive(true);
            UICards[i].card = cards[i];
            StartCoroutine(WaitForObject(UICards[i].gameObject));//va demasiado rapido y el objeto a lo mejor no esta activo
            UICards[i].BuildCard();
        }
            //EventSystem.current.SetSelectedGameObject(UICards[0].gameObject); no se puede hacer porque se fastidia
        

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
        previousInGameState = inGameStates;
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

        //y comienza la noche >:)
        LevelManager.Instance.StartNight();
    }
    //recibir el canvas padre world space y pasar la pos al canvas screen space de player
    public void PassWorldPosToUI(GameObject uiWorld, Canvas WorldCanvas)//el canvas screen space es el de player hud
    {
        MoveUIBetweenCanvases mover = GetComponent<MoveUIBetweenCanvases>();
        mover.rectTransform = uiWorld.GetComponent<RectTransform>();
        if (mover.worldCamera == null)
        {
            mover.worldCamera = Camera.main;
        }
        mover.worldCanvas = WorldCanvas;
        mover.screenCanvas = PlayerHUD.GetComponent<Canvas>();
        mover.MoveToScreenCanvas();
        //emparentar y mover
        CardObject card = uiWorld.GetComponent<CardObject>();
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
        card.transform.SetAsFirstSibling();
        card.GetComponent<CardAnimation>().MoveToCurve(card.GetComponent<RectTransform>(), card.transform.parent.position);
        card.GetComponent<CardAnimation>().Scale(card.GetComponent<RectTransform>(), 2f);
        card.GetComponent<CardAnimation>().RotateXValue(card.GetComponent<RectTransform>(), 0f);
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
        GameObject.Find("CanvasMainMenu/PanelMainMenu/Buttons/OptionsButton").GetComponent<Button>().onClick.AddListener(ShowTabCanvasInMainMenu);
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
    public GameObject PauseMenu;//padre
    [SerializeField]
    List<UISettingsElement> uiElements = new List<UISettingsElement>();//hijos
    [SerializeField]
    bool isSettingsCanvasDirty = false;
    public void InitPauseMenu()
    {
        if (PauseMenu != null)
        {
            DontDestroyOnLoad(PauseMenu);
            PauseMenu.SetActive(false);
            //Asignar funciones a los botones del menu de pausa(selectionCanvas)
            PauseMenu.transform.Find("SelectionCanvas/Continue").GetComponent<Button>().onClick.AddListener(GameManager.Instance.UnPauseGame);
            PauseMenu.transform.Find("SelectionCanvas/Settings").GetComponent<Button>().onClick.AddListener(ShowTabCanvas);
            PauseMenu.transform.Find("SelectionCanvas/Quit").GetComponent<Button>().onClick.AddListener(GoBackToMainMenu);
            uiElements.AddRange(PauseMenu.transform.GetComponentsInChildren<UISettingsElement>(true));
            foreach (var element in uiElements)
            {
                element.Init();
                switch (element.DataType)
                {
                    case VALUE_TYPE.BOOL:
                        element.Subscribe<bool>(ChangeTemporalData);
                        break;
                    case VALUE_TYPE.FLOAT:
                        element.Subscribe<float>(ChangeTemporalData);
                        break;
                    case VALUE_TYPE.STRING:
                        element.Subscribe<string>(ChangeTemporalData);
                        break;
                }
            }
        }
    }
    public void ChangeBoolTemporalData(string uiName, bool value)
    {
        ChangeTemporalData<bool>(uiName, value);
    }
    public void ChangeTemporalData<T>(string uiName, T value)
    {
        //var dataValue= FindAnyObjectByType<Character.Settings.Settings>().GetValue<T>(uiName);
        //Debug.Log("[UIManager]Cambiando el valor en " + uiName + " : " + value);//ya sabemos que funciona
        //Decir a settings que cambie valor y aplique(pero de momento no guarda)
        PauseMenu.transform.Find("TabCanvas/SaveText/SaveImage").GetComponent<Image>().color = Color.red;
        SettingsManager.Instance.SetValue<T>(uiName, value);
        isSettingsCanvasDirty = true;
    }

    public void SaveTemporalData()
    {//Guardar los cambios (avisar a settingsmanager)
        if (!isSettingsCanvasDirty) return;
        SettingsManager.Instance.SaveData();
        PauseMenu.transform.Find("TabCanvas/SaveText/SaveImage").GetComponent<Image>().color = Color.green;

        isSettingsCanvasDirty = false;
    }
    public void DiscardTemporalData()
    {//Descartar los cambios(que settingsmanager haga un load de lo viejo en ALoader y avise de los cambios realizados)
        if (!isSettingsCanvasDirty) return;
        SettingsManager.Instance.LoadData();
        isSettingsCanvasDirty = false;
    }
    public void SetDialog()
    {
        previousInGameState = inGameStates;
        inGameStates = InGameStates.INDIALOG;
    }
    public void CloseDialog()
    {
        previousInGameState = inGameStates;
        inGameStates = InGameStates.INGAME;
    }
    public void OnPauseUI(bool isPaused)
    {
        if (inGameStates == InGameStates.SELECTINGCARDS)
        {
            GameManager.Instance.BlockPause();
            return;//selectingcards es crucial y bloquea la pausa
        }
        //aqui entra en juego el estado previo
        PauseMenu.SetActive(isPaused);
        if (isPaused)
        {
            EventSystem.current.SetSelectedGameObject(PauseMenu.transform.Find("SelectionCanvas/Continue").gameObject);
        }
        //si en pausa:
        //sacar seleccion de tres
        //enseñar cartas(dejar para mas tarde)
        //previous ingame se pone dentro de todos los if porque necesito saber que era antes, y luego actualizo
        if (isPaused && inGameStates == InGameStates.INGAME)//si viene de ingame normal
        {
            Debug.Log("Pausando desde ingame");

            previousInGameState = inGameStates;

            inGameStates = InGameStates.INPAUSE;

            InputManager.Instance.SwitchMapToUI();
            ShowSelectionCanvas();
        }
        else if (isPaused && inGameStates == InGameStates.INDIALOG)//si viene del dialogo
        {
            Debug.Log("Pausando desde dialogo");
            previousInGameState = inGameStates;

            inGameStates = InGameStates.INPAUSE;
            ShowSelectionCanvas();
        }
        //si no en pausa:
        else if (!isPaused && previousInGameState == InGameStates.INDIALOG)
        {
            //no cambiar al mapa de player porque seguimos en interfaz
            Debug.Log("Volviendo a dialogo");
            previousInGameState = inGameStates;

            inGameStates = InGameStates.INDIALOG;
        }
        else if (!isPaused && previousInGameState == InGameStates.INGAME)
        {
            Debug.Log("Volviendo a ingame");
            previousInGameState = inGameStates;

            inGameStates = InGameStates.INGAME;

            InputManager.Instance.SwitchMapToPlayer();
        }
        else
        {
            Debug.LogError("Estado no reconocido en pausa");    
        }



    }
    public void ShowSelectionCanvas()
    {
        isSettingsCanvasDirty = false;
        PauseMenu.transform.Find("SelectionCanvas").gameObject.SetActive(true);
        PauseMenu.transform.Find("TabCanvas").gameObject.SetActive(false);
    }
    public void ShowTabCanvasInMainMenu()
    {
        PauseMenu.SetActive(true);
        ShowTabCanvas();
    }
    public void HideTabCanvasInMainMenu()
    {
        PauseMenu.SetActive(false);
    }
    public void ShowTabCanvas()
    {
        isSettingsCanvasDirty = false;//empieza en true porque carga los cambios
        PauseMenu.transform.Find("SelectionCanvas").gameObject.SetActive(false);
        PauseMenu.transform.Find("TabCanvas").gameObject.SetActive(true);
        PauseMenu.transform.Find("TabCanvas/SaveText/SaveImage").GetComponent<Image>().color = Color.green;

    }
    public void GoBackToMainMenu()
    {
        GameManager.Instance.GoBackToMainMenu();
    }
    #endregion
    #region ManagerLogic
    public void LoadData()
    {
        //Buscar al settingsManager para que me de lo que necesito en los uiElements
        foreach (var element in uiElements)
        {
            switch (element.GetComponent<UISettingsElement>().DataType)
            {
                case VALUE_TYPE.BOOL:
                    //Debug.Log("[CanvasManager] Poniendo valor de "+element.name+" a "+settingsValues.GetValue<bool>(element.name));
                    element.GetComponent<Toggle>().isOn = SettingsManager.Instance.GetValue<bool>(element.name);
                    break;
                case VALUE_TYPE.FLOAT:
                    element.GetComponent<Slider>().value = SettingsManager.Instance.GetValue<float>(element.name);
                    break;
                case VALUE_TYPE.STRING:

                    // string[] parts = settingsValues.GetValue<string>(element.name).Split("::");
                    // string[] actionName=null;

                    // if(parts.Length>=3)
                    //     actionName = parts[2].Split("/");//parts 2 es el binding path<Keyboard>/W por ejemplo

                    // string actionValue = null;

                    // if (actionName.Length > 0)                                         
                    //     actionValue= actionName[1];
                    // if(actionValue!=null)
                    //     element.GetComponent<RebindActionUI>().bindingText.text = actionValue;

                    break;
            }
        }
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
            uiCard.SetActive(false);

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
        InitPauseMenu();
        //Player
        if (PlayerHUD != null)
        {
            DontDestroyOnLoad(PlayerHUD);//quiero que se mantenga la player HUD 
            PlayerHUD.SetActive(false);
            ContinueButton.onClick.AddListener(onEndSelection);

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
        LoadData();
    }





    #endregion
}