using Managers;
using Patterns.Singleton;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;


public class InputManager : ASingleton<InputManager>, IManager
{
    [SerializeField] private PlayerInput playerInput;

    public PlayerInput Input
    {
        get { return playerInput; }
    }

    public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;

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
        throw new System.NotImplementedException();
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
        playerInput.SwitchCurrentActionMap("Player");

    }
    [ContextMenu("Cambiar a mapa de accion UI")]
    public void SwitchMapToUI()
    {
        playerInput.SwitchCurrentActionMap("UI");

    }
    public void StartManager()
    {
         Debug.Log($"[{name}]:Iniciando...");
        SwitchMapToUI();
    }

    public void ResetUIInPutModule(GameObject Button)
    {
        GetComponent<InputSystemUIInputModule>().enabled = false;
        GetComponent<InputSystemUIInputModule>().enabled = true;
        EventSystem.current.SetSelectedGameObject(Button);
    }
}
