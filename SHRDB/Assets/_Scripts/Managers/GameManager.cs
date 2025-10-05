using Patterns.Singleton;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Managers.GameSceneManager;

namespace Managers
{
    public enum GameState { STARTING,INMAINMENU,INGAME,INPAUSE,ENDGAME}
    
    public class GameManager : ASingleton<GameManager>, IManager
    {

        public List<IManager> managersList;
        private GameState gameState=GameState.STARTING;
        public static Action<bool> onPause;
        public GameState CurrentState {  get { return gameState; } }
        void Start()
        {
            SceneManager.sceneLoaded += OnChangeScene;
            if (managersList == null)
            {
                managersList = new List<IManager>();
                var allManagers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                             .OfType<IManager>()
                             .Where(m => !(m is GameManager)); // excluye GameManager

                managersList.AddRange(allManagers);
            }
            StartManager();
        }

        private void OnChangeScene(Scene scene, LoadSceneMode mode)
        {
            switch (scene.buildIndex)
            {
                case (int)SceneIds.MAINMENUSCENE:
                    gameState=GameState.INMAINMENU;
                    break;
                case (int)SceneIds.GAMESCENE: 
                    gameState = GameState.INGAME;
                    OnStartGame();
                    break;
            }
        }
        public void PauseGame()
        {
            gameState=GameState.INPAUSE;
            onPause.Invoke(true);
        }
        public void UnPauseGame()
        {
            gameState = GameState.INGAME;
            onPause.Invoke(false);
        }

       
        
        public void LoadData()
        {

        }

        public void OnEnd()
        {
            foreach (var manager in managersList)
            {
                manager.OnEnd();
            }
        }

        public void OnEndGame()
        {
            foreach (var manager in managersList)
            {
                manager.OnEndGame();
            }
        }

        public void SaveData()
        {
            throw new System.NotImplementedException();
        }

        public void StartManager()
        {
            Debug.Log($"[{name}]:Iniciando...");

            foreach (var manager in managersList)
            {
                manager.StartManager();
            }
            LoadData();
        }

        public void OnStartGame()
        {
            Debug.Log($"[{name}]Empezando juego");
            foreach(var manager in managersList)
            {
                manager.OnStartGame();
            }
        }
    }
}