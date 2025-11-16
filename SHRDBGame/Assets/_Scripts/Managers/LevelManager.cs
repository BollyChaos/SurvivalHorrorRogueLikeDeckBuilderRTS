using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Patterns.Singleton;
using UnityEngine.Events;
using Unity.VisualScripting;
public class LevelManager : ASingleton<LevelManager>, IManager
{
    public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;
    [SerializeField] private int nNights = 5;
    [SerializeField] private int currentNight = 1;
    public int CurrentNigh { get => currentNight; }
    [Tooltip("Duration of the night in seconds")]
    [SerializeField] public float nightDuration = 10f;
    public float NightDuration { get { return nightDuration; } }
    private float nightTimer = 0f;
    [SerializeField] private bool isNightActive = false;
    [SerializeField] public UnityEvent<bool> onNightStateChanged;
    // Update is called once per frame
    void Update()
    {
        if (!isNightActive) return;
        nightTimer += Time.deltaTime;
        if (nightTimer >= nightDuration)
        {
            EndNight();
        }
    }

    public void StartNight()
    {
        isNightActive = true;
        onNightStateChanged?.Invoke(isNightActive);
    }
    public void EndNight()
    {
        isNightActive = false;
        nightTimer = 0f;
        onNightStateChanged?.Invoke(isNightActive);

    }
    [ContextMenu("Next Night")]
    public void NextNight()
    {
        if (currentNight < nNights)
        {
            currentNight++;
            StartNight();
        }
        else
        {
            Debug.Log("All nights completed!");
            WinGame();
            // Aquí puedes agregar la lógica para cuando se completen todas las noches
        }
    }
    public void EndGame()
    {
        Debug.Log("Fin de la partida");
        InputManager.Instance.SwitchMapToUI();
        //llamar al uimanager tambien

        UIManager.Instance.EndGame();
        SoundManager.Instance.OnPlayerDeath();
        //Desactivar enemigos
    }
    public void WinGame()
    {
          Debug.Log("Fin de la partida");
        InputManager.Instance.SwitchMapToUI();
        //llamar al uimanager tambien

        UIManager.Instance.EndGame(true);
    }
    #region MANAGERLOGIC
    public void LoadData()
    {
        //aqui se guardaran las variables relativas al contexto del juego, por si un jugador se sale en mitad de la partida
    }

    public void OnEnd()
    {
    }

    public void OnEndGame()
    {
    }

    public void OnStartGame()
    {
    }

    public void SaveData()
    {
    }

    public void StartManager()
    {
    }
    #endregion

}
