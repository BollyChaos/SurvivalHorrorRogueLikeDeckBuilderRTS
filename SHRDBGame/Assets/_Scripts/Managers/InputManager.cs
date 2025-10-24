using Managers;
using Patterns.Singleton;
using System;
using System.IO;
using UI.Tabs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


public class InputManager : ASingleton<InputManager>, IManager
{
    [SerializeField] private PlayerInput playerInput;
    public enum InputMap{PLAYER,UI}
    public InputMap inputMap = InputMap.PLAYER;
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
        }
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
    }

    public void OnStartGame()
    {
        SwitchMapToUI();
        

    }

    public void SaveData()
    {
        throw new System.NotImplementedException();
    }
    [ContextMenu("Cambiar a mapa de accion Player")]
    public void SwitchMapToPlayer()
    {
        inputMap = InputMap.PLAYER;
        playerInput.SwitchCurrentActionMap("Player");

    }
    [ContextMenu("Cambiar a mapa de accion UI")]
    public void SwitchMapToUI()
    {
        inputMap = InputMap.UI;
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
        //La pausa segun el mapa
        playerInput.actions.FindActionMap("Player").FindAction("Escape").started+=OnEscape;
        playerInput.actions.FindActionMap("Player").FindAction("Escape").performed+=OnEscape;
        playerInput.actions.FindActionMap("UI").FindAction("Escape").started+=OnEscape;
        playerInput.actions.FindActionMap("UI").FindAction("Escape").performed+=OnEscape;
    }

    public void ResetUIInPutModule(GameObject Button=null)
    {
        GetComponent<InputSystemUIInputModule>().enabled = false;
        GetComponent<InputSystemUIInputModule>().enabled = true;
        if (Button != null)
        EventSystem.current.SetSelectedGameObject(Button);
    }
    #endregion
}
