using Character.Settings;
using Managers;
using Patterns.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Managers.GameSceneManager;
using static Managers.IManager;
public class UIManager : ASingleton<UIManager>, IManager
{
    //GameManager tiene sus estados pero no le importa en que estado se esta dentro del juego, ahi es donde entra uiManager 
    public enum InGameStates { INGAME, INDIALOG, INPAUSE, SELECTINGCARDS, DAYTIME, ENDGAME }//ya se que gamemanager tiene inpause y no creo que sea redundante ya que uimanager necesita saber si esta en pausa
    public GameStartMode StartMode => GameStartMode.EARLY;
    [SerializeField] InGameStates previousInGameState;
    [SerializeField] InGameStates inGameStates;

    [Header("Player")]
    [SerializeField]
    GameObject PlayerHUD;
    [SerializeField]
    GameObject MobileHUD;


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
        // Debug.Log("Cartas que llegan: "+cards.Count+",cartas de interfaz: "+UICards.Count);
        for (int i = 0; i < UICards.Count; i++)
        {
            Debug.Log("Construyendo Cartas");
            UICards[i].gameObject.SetActive(true);
            UICards[i].card = cards[i];
            //StartCoroutine(WaitForObject(UICards[i].gameObject));//va demasiado rapido y el objeto a lo mejor no esta activo
            UICards[i].BuildCard();
        }
        //EventSystem.current.SetSelectedGameObject(UICards[0].gameObject); no se puede hacer porque se fastidia
        PlayerHUD.transform.Find("CardsSelector").gameObject.SetActive(true);

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
        PlayerHUD.transform.Find("CardsSelector").gameObject.SetActive(false);
        ContinueButton.gameObject.SetActive(false);
        previousInGameState = inGameStates;
        inGameStates = InGameStates.INGAME;


        foreach (var card in UICards)
        {
            string path = "";

            Transform parent = PlayerHUD.transform.Find("CardsDisplay");

            switch (card.card.cardType)
            {
                case CardType.Attack:
                    path = "CardsDisplay/LeftCard";
                    break;
                case CardType.Defense:
                    path = "CardsDisplay/CenterCard";
                    break;
                case CardType.Utility:
                    path = "CardsDisplay/RightCard";
                    break;
            }



            EmparentCard(path, card.gameObject);

            card.GetComponent<SelectableUICard>().MoveToCurve(card.transform.parent.position);
            card.GetComponent<SelectableUICard>().Scale(2f);
        }
        foreach (var card in UICards)//poner delante las seleccionadas
        {
            var ui = card.GetComponent<SelectableUICard>();
            if (ui.isOn)
            {
                string path = "";
                switch (card.card.cardType)
                {
                    case CardType.Attack:
                        path = "CardsDisplay/LeftCard";
                        break;
                    case CardType.Defense:
                        path = "CardsDisplay/CenterCard";
                        break;
                    case CardType.Utility:
                        path = "CardsDisplay/RightCard";
                        break;
                }
                EmparentCard(path, card.gameObject, 0);
            }
            card.GetComponent<SelectableUICard>().NextCardPhase();
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
        //Movido al fin de dialogo con Eleanor
        GameObject.Find("Eleanor")?.GetComponent<EleanorBehaviour>().EndCardSelection();
    }
    //recibir el canvas padre world space y pasar la pos al canvas screen space de player
    public void PassWorldPosToUI(GameObject uiWorld, Canvas WorldCanvas)//el canvas screen space es el de player hud
    {
        MoveUIBetweenCanvases mover = GetComponent<MoveUIBetweenCanvases>();
        mover.rectTransform = uiWorld.GetComponent<RectTransform>();
        if (mover.worldCamera == null)
        {
            mover.worldCamera = FindAnyObjectByType<CameraController>().PlayerCamera;
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
    void EmparentCard(string objectName, GameObject objectToMove, int siblingIndex = -1)
    {
        Transform parent = PlayerHUD.transform.Find(objectName);

        if (parent != null && objectToMove != null)
        {
            objectToMove.transform.SetParent(parent, true);

            // Si siblingIndex >= 0, lo aplicamos
            if (siblingIndex == 0)
            {
                objectToMove.transform.SetAsLastSibling();
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró '{objectName}' o 'objectToMove' no está asignado");
        }
    }

    CardObject FindLastCard(string objectName)
    {
        Transform parent = PlayerHUD.transform.Find(objectName); // o el que necesites
        if (parent.childCount > 0)
        {
            Transform ultimoHijo = parent.GetChild(parent.childCount - 1);
            //            Debug.Log("El �ltimo hijo es: " + ultimoHijo.name);
            return ultimoHijo.GetComponent<CardObject>();
        }
        return null;
    }
    #endregion
    #region PLAYERUI
    public void ShowMobileInput()
    {

        MobileHUD.SetActive(true);
        ShowJoystick();
    }
    public void HideMobileInput()
    {
        MobileHUD.SetActive(false);

    }
    public void HideJoystickEndGame()
    {
        MobileHUD.transform.Find("LeftStick").gameObject.SetActive(false);
    }

    public void HideJoystick()
    {
        MobileHUD.transform.Find("LeftStick").gameObject.SetActive(false);
        DialogManager.Instance.onEndDialog.AddListener(ShowJoystick);
    }
    public void ShowJoystick()
    {
        MobileHUD.transform.Find("LeftStick").gameObject.SetActive(true);
        DialogManager.Instance.onEndDialog.RemoveListener(ShowJoystick);
    }
    public void ShowSkipTutorialButton()
    {
        Button skipButton = PlayerHUD.transform.Find("Skip").GetComponent<Button>();
        skipButton.gameObject.SetActive(true);
        skipButton.onClick.AddListener(SkipTutorialButton);

    }
    private void SkipTutorialButton()
    {
        FindAnyObjectByType<EleanorBehaviour>().SkipTutorial();

    }
    public void HideSkipTutorialButton()
    {
        Button skipButton = PlayerHUD.transform.Find("Skip").GetComponent<Button>();
        skipButton.onClick.RemoveAllListeners();
        skipButton.gameObject.SetActive(false);

    }
    public void SetPlayerHealthUI(float healthAmmount)
    {
        PlayerHUD.transform.Find("PlayerHealth").GetComponent<HealthBarSlider>().SetHealth(healthAmmount);
    }
    public void SetInteractionText(string text)
    {
        //Debug.Log("Hola");
        PlayerHUD.transform.Find("InteractionText").gameObject.SetActive(true);
        PlayerHUD.transform.Find("InteractionText").GetComponent<TextMeshProUGUI>().text = text;
    }
    public void HideInteractionText()
    {
        //Debug.Log("Adios");
        PlayerHUD.transform.Find("InteractionText").gameObject.SetActive(false);

    }
    public void HideCardsDialog()
    {
        PlayerHUD.transform.Find("CardsDisplay").gameObject.SetActive(false);
        DialogManager.Instance.onEndDialog.AddListener(ShowCardsDialog);
    }
    public void ShowCardsDialog()
    {
        PlayerHUD.transform.Find("CardsDisplay").gameObject.SetActive(true);
        DialogManager.Instance.onEndDialog.RemoveListener(ShowCardsDialog);
    }
    public void ShowRoomText(string roomText)
    {
        PlayerHUD.transform.Find("RoomText").GetComponent<TextMeshProUGUI>().text = roomText;
    }
    public void HideRoomText(string roomText)
    {

        if (PlayerHUD.transform.Find("RoomText").GetComponent<TextMeshProUGUI>().text == roomText)
        {
            PlayerHUD.transform.Find("RoomText").GetComponent<TextMeshProUGUI>().text = "";
        }


    }
    public void ShowMoneyForAWhile(int money)
    {
        GameObject moneyObj = PlayerHUD.transform.Find("PlayerMoney").gameObject;
        if (moneyObj == null)
        {
            Debug.LogError("No se encuentra PlayerMoney");
            return;
        }
        moneyObj.SetActive(true);

        //PlayerHUD.transform.Find("PlayerMoney").GetComponent<TextMeshProUGUI>().text = $"x{money}";
        var text = moneyObj.GetComponent<TextMeshProUGUI>();
        text.text = $"x{money}";

        StartCoroutine(FadeOutText(text, moneyObj));
    }
    IEnumerator FadeOutText(TextMeshProUGUI text, GameObject obj, float time = 1.5f)
    {
        // Espera visible
        yield return new WaitForSeconds(time);

        Color c = text.color;
        float t = 0f;
        while (t < 0.75f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / 0.75f);
            text.color = c;
            yield return null;
        }
        c.a = 1f;
        text.color = c;
        obj.SetActive(false);

    }
    public void ShowGameEventForAWhile(string eventText)
    {
        GameObject eventObj = PlayerHUD.transform.Find("EventText").gameObject;
        if (eventObj == null)
        {
            Debug.LogError("No se encuentra PlayerMoney");
            return;
        }
        eventObj.SetActive(true);

        //PlayerHUD.transform.Find("PlayerMoney").GetComponent<TextMeshProUGUI>().text = eventText;
        var text = eventObj.GetComponent<TextMeshProUGUI>();
        text.text = eventText;

        StartCoroutine(FadeOutText(text, eventObj, 3.5f));
    }



    public void ShowMoney(int money)
    {
        PlayerHUD.transform.Find("PlayerMoney").gameObject.SetActive(true);
        PlayerHUD.transform.Find("PlayerMoney").GetComponent<TextMeshProUGUI>().text = $"x{money}";
    }
    public void HideMoney()
    {
        PlayerHUD.transform.Find("PlayerMoney").gameObject.SetActive(false);
    }
    #endregion

    #region ENDGAME
    [Header("EndGameUI")]
    public GameObject EndGameCavas;
    public void EndGame(bool won = false)
    {
        previousInGameState = InGameStates.ENDGAME;
        inGameStates = InGameStates.ENDGAME;
        if (won)
        {
            EndGameCavas.transform.Find("EndGameText").GetComponent<TextMeshProUGUI>().text = "¡Has ganado!";
        }
        else
        {
            EndGameCavas.transform.Find("EndGameText").GetComponent<TextMeshProUGUI>().text = "¡Has muerto!";

        }
        HideJoystick();
        ShowEndGameCanvas();
    }
    internal void ShowEndGameCanvas()
    {

        EndGameCavas.SetActive(true);
    }


    #endregion

    #region MainMenu
    [Header("MainMenu")]

    public Button PlayButton;
    public GameObject Credits;
    void OnPlayPressed()
    {
        GameSceneManager.Instance.LoadSceneById((int)GameSceneManager.SceneIds.GAMESCENE);
    }

    internal void LookForMainMenuCanvas()
    {
        PlayButton = GameObject.Find("CanvasMainMenu/PanelMainMenu/Buttons/PlayButton").GetComponent<Button>();
        Credits = GameObject.Find("CanvasMainMenu/Credits");
        Credits.transform.SetAsLastSibling();
        Credits.SetActive(false);
        GameObject.Find("CanvasMainMenu/PanelMainMenu/Buttons/OptionsButton").GetComponent<Button>().onClick.AddListener(ShowTabCanvasInMainMenu);
        GameObject.Find("CanvasMainMenu/PanelMainMenu/Buttons/CreditsButton").GetComponent<Button>().onClick.AddListener(ShowCredits);
        GameObject.Find("CanvasMainMenu/PanelMainMenu/Buttons/ExitButton").GetComponent<Button>().onClick.AddListener(QuitApplication);


        //  Debug.Log(PlayButton == null);

        if (PlayButton != null)
        {
            PlayButton.onClick.AddListener(OnPlayPressed);
            //Resetear uiinputmodule por si se ralla
            InputManager.Instance.ResetUIInPutModule(PlayButton.gameObject);
        }
    }
    public void HideCredits()
    {
        GameManager.Instance.OutCredits();
        Credits.gameObject.SetActive(false);
        SoundManager.Instance.PlayTrack(SoundManager.SoundTrack.MENU);
    }
    public void ShowCredits()
    {
        GameManager.Instance.InCredits();
        Credits.gameObject.SetActive(true);
        SoundManager.Instance.PlayTrack(SoundManager.SoundTrack.CREDITS);

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
            PauseMenu.transform.Find("SelectionCanvas/Buttons/Continue").GetComponent<Button>().onClick.AddListener(GameManager.Instance.UnPauseGame);
            PauseMenu.transform.Find("SelectionCanvas/Buttons/Settings").GetComponent<Button>().onClick.AddListener(ShowTabCanvas);
            PauseMenu.transform.Find("SelectionCanvas/Buttons/Quit").GetComponent<Button>().onClick.AddListener(GoBackToMainMenu);
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
        //PauseMenu.transform.Find("TabCanvas/SaveText/SaveImage").GetComponent<Image>().color = Color.green; descartado, no me terminaba de convencer

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
        //primero ver si esta muerto

        if (inGameStates == InGameStates.SELECTINGCARDS || inGameStates == InGameStates.ENDGAME)
        {
            GameManager.Instance.BlockPause();
            return;//selectingcards es crucial y bloquea la pausa
        }
        //aqui entra en juego el estado previo
        PauseMenu.SetActive(isPaused);
        if (isPaused)
        {
            EventSystem.current.SetSelectedGameObject(PauseMenu.transform.Find("SelectionCanvas/Buttons/Continue").gameObject);
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
            PlayerHUD.SetActive(true);

            inGameStates = InGameStates.INDIALOG;
        }
        else if (!isPaused && previousInGameState == InGameStates.INGAME)
        {

            Debug.Log("Volviendo a ingame");
            previousInGameState = inGameStates;


            inGameStates = InGameStates.INGAME;
            PlayerHUD.SetActive(true);

            InputManager.Instance.SwitchMapToPlayer();
        }
        else
        {
            Debug.LogError("Estado no reconocido en pausa");
        }
        SaveTemporalData();



    }
    public void ShowSelectionCanvas()
    {
        PlayerHUD.SetActive(true);
        //mas avanzado, enseñar todas las cartas
        isSettingsCanvasDirty = false;
        PauseMenu.transform.Find("SelectionCanvas").gameObject.SetActive(true);
        PauseMenu.transform.Find("TabCanvas").gameObject.SetActive(false);
        //cartas
        ShowActiveCards();
    }
    void ShowActiveCards()
    {
        CardUser playerCardUser = FindAnyObjectByType<CardUser>();
        if (playerCardUser.HasAnyCards)
        {
            Debug.Log("El jugador tiene cartas");
            if (playerCardUser.HasAttackCards)
            {
                Debug.Log("El jugador tiene carta de ataque");
                CardObject attackCard = playerCardUser.GetAttackCard();
                PauseMenu.transform.Find("SelectionCanvas/Cards/UICardAttack").GetComponent<CardObject>().card = attackCard.card;
                PauseMenu.transform.Find("SelectionCanvas/Cards/UICardAttack").GetComponent<CardObject>().BuildCard();
                PauseMenu.transform.Find("SelectionCanvas/Cards/UICardAttack").gameObject.SetActive(true);
            }
            else
            {
                PauseMenu.transform.Find("SelectionCanvas/Cards/UICardAttack").gameObject.SetActive(false);
            }
            if (playerCardUser.HasDefenseCards)
            {
                CardObject defenseCard = playerCardUser.GetDefenseCard();
                PauseMenu.transform.Find("SelectionCanvas/Cards/UICardDefense").GetComponent<CardObject>().card = defenseCard.card;
                PauseMenu.transform.Find("SelectionCanvas/Cards/UICardDefense").GetComponent<CardObject>().BuildCard();
                PauseMenu.transform.Find("SelectionCanvas/Cards/UICardDefense").gameObject.SetActive(true);
            }
            else
            {
                PauseMenu.transform.Find("SelectionCanvas/Cards/UICardDefense").gameObject.SetActive(false);
            }
            if (playerCardUser.HasUtilityCards)
            {
                CardObject utilityCard = playerCardUser.GetUtilityCard();
                PauseMenu.transform.Find("SelectionCanvas/Cards/UICardUtility").GetComponent<CardObject>().card = utilityCard.card;
                PauseMenu.transform.Find("SelectionCanvas/Cards/UICardUtility").GetComponent<CardObject>().BuildCard();
                PauseMenu.transform.Find("SelectionCanvas/Cards/UICardUtility").gameObject.SetActive(true);
            }
            else
            {
                PauseMenu.transform.Find("SelectionCanvas/Cards/UICardUtility").gameObject.SetActive(false);
            }
        }
        else
        {
            PauseMenu.transform.Find("SelectionCanvas/Cards/UICardAttack").gameObject.SetActive(false);
            PauseMenu.transform.Find("SelectionCanvas/Cards/UICardDefense").gameObject.SetActive(false);
            PauseMenu.transform.Find("SelectionCanvas/Cards/UICardUtility").gameObject.SetActive(false);
        }
    }
    public void ShowTabCanvasInMainMenu()
    {
        PauseMenu.SetActive(true);
        ShowTabCanvas();
    }
    public void HideTabCanvasInMainMenu()
    {
        PauseMenu.SetActive(false);
        SaveTemporalData();
    }

    public void ShowTabCanvas()
    {
        PlayerHUD.SetActive(false);
        isSettingsCanvasDirty = false;//empieza en true porque carga los cambios
        PauseMenu.transform.Find("SelectionCanvas").gameObject.SetActive(false);
        PauseMenu.transform.Find("TabCanvas").gameObject.SetActive(true);
        PauseMenu.transform.Find("TabCanvas/SaveText/SaveImage").GetComponent<Image>().color = Color.green;

    }
    public void GoBackToMainMenu()
    {
        inGameStates = InGameStates.ENDGAME;
        previousInGameState = InGameStates.ENDGAME;

        Debug.Log("Cerrando");
        PlayerHUD.SetActive(false);
        Debug.Log(PlayerHUD.activeSelf);
        EndGameCavas.SetActive(false);
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



    public void OnEndGame()
    {
        //TODO eliminar cartas de player hud(o quizas guardarlas para la proxima partida?->otro metodo para guardar preguntar si se quiere guardar partida antes de salir)
        //quitar cartas de player HUD
        //0. desactivar segun la plataforma
        EndUIPlatform();
        previousInGameState = InGameStates.INGAME;
        inGameStates = InGameStates.INGAME;
        //quitar pausa al acabar juego
        OnPauseUI(false);
        //quitar la interfaz de usuario
        PlayerHUD.SetActive(false);
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
        inGameStates = InGameStates.INGAME;
        previousInGameState = inGameStates;
        //0. Ver la plataforma en la que se arranca
        PrepareUIPlatform();
        //1.Al empezar juego se activa la hud del player
        Debug.Log($"[{name}]Empezando juego");
        PauseMenu?.SetActive(false);
        PlayerHUD?.SetActive(true);
        //fijar la camara en offscreenindicator
        FindAnyObjectByType<OffScreenIndicator>().SetCamera(FindAnyObjectByType<CameraController>().PlayerCamera);
        EndGameCavas?.SetActive(false);

        //2.UI Cards
        //Ahora queremos instanciar las cartas y manejarlo de forma dinamica para poder tener bien el estado 0 del juego
        //esto es un poco xd habría que refactorizar:
        //a lo mejor crear n Car{i+1} y dentro con el prefab emparentarlo, pero al menos funciona asi
        string parent = "CardsSelector";

        for (int i = 0; i < CardManager.Instance.startingCards; i++)
        {
            string emptyCardHolder = $"Card({i + 1})";
            string path = parent + "/" + emptyCardHolder;

            Transform parentTransform = PlayerHUD.transform.Find(parent);

            // Crear si no existe
            Transform holder = PlayerHUD.transform.Find(path);
            if (holder == null)
            {
                GameObject cardHolder = new GameObject(emptyCardHolder, typeof(RectTransform));

                cardHolder.transform.SetParent(parentTransform, false);
                holder = cardHolder.transform;
            }

            // Crear carta UI
            GameObject uiCard = Instantiate(CardPrefab);
            uiCard.transform.SetParent(holder, false);

            uiCard.GetComponent<RectTransform>().localScale = Vector3.one * 3f;
            uiCard.SetActive(false);
        }
        PlayerHUD.transform.Find(parent).gameObject.SetActive(false);
    }
    public void PrepareUIPlatform()
    {
        switch (GameManager.Instance.gamePlatform)
        {
            case GamePlatform.WebGL_Mobile:
                HideMobileInput();
                ShowMobileInput();
                break;
            case GamePlatform.WebGL_PC:
            case GamePlatform.Standalone:
                HideMobileInput();
                break;
        }
    }
    public void EndUIPlatform()
    {
        switch (GameManager.Instance.gamePlatform)
        {
            case GamePlatform.WebGL_Mobile:
                HideMobileInput();
                break;
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
        //movil
        if (MobileHUD != null)
        {
            DontDestroyOnLoad(MobileHUD);
        }
        //Shop
        if (EndGameCavas != null)
        {
            DontDestroyOnLoad(EndGameCavas);//solo va a existir dentro del juego pero quiero mantenerlo tambien por sea caso
            EndGameCavas.SetActive(false);
            //conectar los tres botones
            EndGameCavas.transform.Find("EndGameText/Exit").GetComponent<Button>().onClick.AddListener(QuitApplication);
            EndGameCavas.transform.Find("EndGameText/BackToMainMenu").GetComponent<Button>().onClick.AddListener(GoBackToMainMenu);
            EndGameCavas.transform.Find("EndGameText/Reset").GetComponent<Button>().onClick.AddListener(GameManager.Instance.RestartGame);

        }
        LoadData();
    }
    public void OnEnd()
    {
        Debug.Log($"[{name} cerrando...]");
        //Player
        ContinueButton.onClick.RemoveAllListeners();
        //End Game
        EndGameCavas.transform.Find("EndGameText/Exit").GetComponent<Button>().onClick.RemoveAllListeners();
        EndGameCavas.transform.Find("EndGameText/BackToMainMenu").GetComponent<Button>().onClick.RemoveAllListeners();
        EndGameCavas.transform.Find("EndGameText/Reset").GetComponent<Button>().onClick.RemoveAllListeners();

    }




    #endregion
}