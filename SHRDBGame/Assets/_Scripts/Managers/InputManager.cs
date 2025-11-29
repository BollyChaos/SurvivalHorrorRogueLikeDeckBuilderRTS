using Managers;
using Patterns.Singleton;
using UI.Tabs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using System.Linq;


public class InputManager : ASingleton<InputManager>, IManager
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputSystemUIInputModule uiModule;
    public enum InputMap{PLAYER,UI,MOBILE}
    public InputMap inputMap = InputMap.PLAYER;
    [SerializeField]
    public PlayerInput Input
    {
        get { return playerInput; }
    }

    public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;
    #region GAMEINPUTS
    //el input manager manejara esta opcion ya que necesitara ver el estado del juego
    //si esta en el mainmenu, pues igual a salir del juego
    //si esta ingame:
    //ver si esta en pausa: donde del menu de pausa especificamente
    //si esta jugando->llevar al menu de pausa
    //si esta en mitad de un dialogo, volver al dialogo
    public void OnEscape(InputAction.CallbackContext ctx)
    {
        //TODO LOGICA DE LA PAUSA  
        switch (GameManager.Instance.CurrentState)
        {
            case GameState.INGAME:
                GameManager.Instance.PauseGame();
                break;
            case GameState.INPAUSE:
                GameManager.Instance.UnPauseGame();
                break;
            case GameState.INMAINMENU:
                UIManager.Instance.HideTabCanvasInMainMenu();
                break;
            case GameState.INCREDITS:
                GameManager.Instance.OutCredits();
                UIManager.Instance.HideCredits();
            //cerrar creditos
            break;
        }
    }
    // public void OnSaveChanges(InputAction.CallbackContext ctx)
    // {
    //     UIManager.Instance.SaveTemporalData();
    // }
    public void ReadDialogInput(InputAction.CallbackContext context)
    {
        if (context.started)
            DialogManager.Instance.ReadInputValue(context.ReadValue<float>() > 0);
        if(context.canceled)
            DialogManager.Instance.ReadInputValue(false);
        
        
    }
    #endregion

#region MANAGERLOGIC
    public void LoadData()
    {
        throw new System.NotImplementedException();
    }

    public void OnEnd()
    {
        Debug.Log($"[{name} cerrando...]");
        GameObject pauseMenu = UIManager.Instance.PauseMenu;
        if (pauseMenu != null)
        {
            GameObject tabs = pauseMenu.transform.Find("TabCanvas/Menu/Tabs").gameObject;
            playerInput.actions["NavigateTabs"].started -= tabs.GetComponent<TabGroup>().OnNavigateTabs;
            playerInput.actions["NavigateTabs"].performed -= tabs.GetComponent<TabGroup>().OnNavigateTabs;
            playerInput.actions["NavigateTabs"].canceled -= tabs.GetComponent<TabGroup>().OnNavigateTabs;
        }
    }

    public void OnEndGame()
    {
        SwitchMapToUI();
    }

    public void OnStartGame()
    {
        switch (GameManager.Instance.gamePlatform)
        {
         case GamePlatform.Standalone:
         case GamePlatform.WebGL_PC:
        SwitchMapToPlayer();
        SwitchUIModule();
         break;   
         case GamePlatform.WebGL_Mobile:
         SwitchMapToMobile();
         SwitchUIModule();
         break;
        }
        

    }
    private void SwitchUIModule()
    {
        switch(GameManager.Instance.gamePlatform)
        {
            case GamePlatform.Standalone:
         case GamePlatform.WebGL_PC:

         break;
          case GamePlatform.WebGL_Mobile:

          break; 
        }
    }
    // private void SwitchUIModule(string map) no funciona
    // {
         
    //     Debug.Log($"UI InputModule usando mapa: {map}");
    //     playerInput.actions.FindActionMap(map)?.Enable();

    //     uiModule.point = InputActionReference.Create(playerInput.actions.FindAction($"{map}/Point"));
    //     uiModule.leftClick = InputActionReference.Create(playerInput.actions.FindAction($"{map}/Click",throwIfNotFound: false));
    //     uiModule.middleClick = InputActionReference.Create(playerInput.actions.FindAction($"{map}/MiddleClick",throwIfNotFound: false));
    //     uiModule.rightClick = InputActionReference.Create(playerInput.actions.FindAction($"{map}/RightClick",throwIfNotFound: false));
    //     uiModule.scrollWheel = InputActionReference.Create(playerInput.actions.FindAction($"{map}/Scroll", throwIfNotFound: false));
    //     uiModule.submit = InputActionReference.Create(playerInput.actions.FindAction($"{map}/Submit"));
    //     uiModule.cancel = InputActionReference.Create(playerInput.actions.FindAction($"{map}/Cancel", throwIfNotFound: false));
    //     uiModule.move = InputActionReference.Create(playerInput.actions.FindAction($"{map}/Navigate", throwIfNotFound: false));

    // }
    public void SaveData()
    {
        throw new System.NotImplementedException();
    }
    [ContextMenu("Cambiar a mapa de accion Player")]
    public void SwitchMapToPlayer()
    {
        if(GameManager.Instance.gamePlatform==GamePlatform.WebGL_Mobile) return;//en movil esto no es necesario, sigue si propio esquema
        
        inputMap = InputMap.PLAYER;
        playerInput.SwitchCurrentActionMap("Player");

    }
    [ContextMenu("Cambiar a mapa de accion UI")]
    
    public void SwitchMapToUI()
    {
        inputMap = InputMap.UI;
        playerInput.SwitchCurrentActionMap("UI");

    }
    public void SwitchMapToMobile()
    {
        inputMap=InputMap.MOBILE;

        playerInput.SwitchCurrentActionMap("UI");
    }
    public void StartManager()
    {
        Debug.Log($"[{name}]:Iniciando...");
        SwitchMapToUI();
        //tambien agregar a tabgroup la accion de navigate tabs
        GameObject pauseMenu = UIManager.Instance.PauseMenu;
        GameObject tabs = pauseMenu.transform.Find("TabCanvas/Menu/Tabs").gameObject;
        playerInput.actions["NavigateTabs"].started+=tabs.GetComponent<TabGroup>().OnNavigateTabs;
        playerInput.actions["NavigateTabs"].performed+=tabs.GetComponent<TabGroup>().OnNavigateTabs;
        playerInput.actions["NavigateTabs"].canceled += tabs.GetComponent<TabGroup>().OnNavigateTabs;
        //agregar accion de guardar, (descartado)
        // playerInput.actions.FindActionMap("UI").FindAction("SaveChanges").started+=OnSaveChanges;
        // playerInput.actions.FindActionMap("UI").FindAction("SaveChanges").performed+=OnSaveChanges;
        //La pausa segun el mapa
        playerInput.actions.FindActionMap("Player").FindAction("Escape").started+=OnEscape;
        playerInput.actions.FindActionMap("Player").FindAction("Escape").performed+=OnEscape;
        playerInput.actions.FindActionMap("UI").FindAction("Escape").started+=OnEscape;
        playerInput.actions.FindActionMap("UI").FindAction("Escape").performed += OnEscape;
        //el input de dialogos

        playerInput.actions.FindActionMap("UI").FindAction("Submit").started += ReadDialogInput;
        playerInput.actions.FindActionMap("UI").FindAction("Submit").canceled += ReadDialogInput;        
    }

    public void ResetUIInPutModule(GameObject Button=null)
    {
        GetComponent<InputSystemUIInputModule>().enabled = false;
        GetComponent<InputSystemUIInputModule>().enabled = true;
        if (Button != null)
        EventSystem.current.SetSelectedGameObject(Button);
    }
    #endregion
    // public void Update()
    // {
    //     if(UnityEngine.Input.GetKeyDown(KeyCode.E)){SwitchMapToPlayer();}
    // }
    // #region DEBUG

    // [Header("Debug")]
    //  public string uiMapName = "UI";
    // public string mobileMapName = "Mobile";
    // public string mapToCheck = "UI"; // o "Mobile"
    // public Canvas canvasToCheck;
    // public Button buttonToCheck;

    //   [ContextMenu("Run UI Input Diagnostic")]
    // public void RunCheck()
    // {
    //     Debug.Log("---- UI INPUT DIAGNOSTIC ----");

    //     var uiModule = GetComponent<InputSystemUIInputModule>();
    //     if (uiModule == null)
    //     {
    //         Debug.LogError("No InputSystemUIInputModule en este GameObject (EventSystem).");
    //         return;
    //     }
    //     Debug.Log("InputSystemUIInputModule: OK");

    //     var actions = uiModule.actionsAsset;
    //     if (actions == null)
    //     {
    //         Debug.LogError("Actions Asset NO asignado en InputSystemUIInputModule.");
    //         return;
    //     }
    //     Debug.Log($"Actions Asset: {actions.name}");

    //     string map = mapToCheck;
    //     var mapObj = actions.FindActionMap(map);
    //     Debug.Log(mapObj != null ? $"Found map '{map}'" : $"Map '{map}' NOT FOUND");

    //     // Acciones comunes
    //     CheckAction(actions, map, "Point", uiModule.point);
    //     CheckAction(actions, map, "Click", uiModule.leftClick);
    //     CheckAction(actions, map, "Submit", uiModule.submit);
    //     CheckAction(actions, map, "Navigate", uiModule.move);

    //     // ¿Está el mapa activo/enabled?
    //     bool mapEnabled = mapObj != null && mapObj.enabled;
    //     Debug.Log($"Map '{map}' enabled: {mapEnabled}");

    //     // Canvas / Raycaster / Button checks
    //     if (canvasToCheck == null)
    //     {
    //         Debug.LogWarning("canvasToCheck no asignado. Salta comprobación de Canvas/GraphicRaycaster.");
    //     }
    //     else
    //     {
    //         var gr = canvasToCheck.GetComponent<GraphicRaycaster>();
    //         Debug.Log(gr != null ? "GraphicRaycaster en canvas: OK" : "GraphicRaycaster NO encontrado en Canvas.");
    //     }

    //     if (buttonToCheck == null)
    //     {
    //         Debug.LogWarning("buttonToCheck no asignado. Salta comprobación de Button.");
    //     }
    //     else
    //     {
    //         Debug.Log($"Button interactable: {buttonToCheck.interactable}");
    //         Debug.Log($"Button GameObject activo: {buttonToCheck.gameObject.activeInHierarchy}");
    //         // check if any parent canvas is enabled
    //         var parentCanvas = buttonToCheck.GetComponentInParent<Canvas>();
    //         Debug.Log(parentCanvas != null ? $"Parent Canvas: {parentCanvas.name}" : "No parent Canvas encontrado.");
    //     }

    //     Debug.Log("---- END DIAGNOSTIC ----");
    // }

    // void CheckAction(InputActionAsset actions, string map, string actionName, InputActionReference reference)
    // {
    //     var fullPath = $"{map}/{actionName}";
    //     var action = actions.FindAction(fullPath, throwIfNotFound:false);
    //     Debug.Log($"{fullPath}: action found = {action != null}, reference assigned = {reference != null && reference.action != null}");
    //     if (action != null)
    //     {
    //         var bindings = action.bindings.Select(b => b.path).Distinct();
    //         Debug.Log($"  Bindings: {string.Join(", ", bindings)}");
    //     }
    // }

    // #endregion
}
