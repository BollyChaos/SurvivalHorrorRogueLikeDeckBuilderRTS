using Managers;
using Patterns.Singleton;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;


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
        throw new System.NotImplementedException();
    }

    public void OnEndGame()
    {
        throw new System.NotImplementedException();
    }

    public void OnStartGame()
    {
        
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
    public void SwitchMapToUI()
    {
        playerInput.SwitchCurrentActionMap("UI");

    }
    public void StartManager()
    {
         Debug.Log($"[{name}]:Iniciando...");
        SwitchMapToUI();
    }
}
