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

        public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;
        #region DEBUGGING
        [Header("DebugGame")]
        public bool DebugGame = true;
        [ShowIf("DebugGame")]
        //Saltar alguna escena
        public bool ChangeInitalScene = true;
        [ShowIf("ChangeInitalScene","DebugGame")]
        public SceneIds startingDebugScene = SceneIds.GAMESCENE;
        //Saltar fase
        [ShowIf("DebugGame")]
        public bool SkipPhase = true;
        [ShowIf("SkipPhase","DebugGame")]
        public bool SkipCardSelectionPhase=true;

        #endregion

        void Start()
        {
            //primero logica debug
            if (DebugGame)
            {
                if (ChangeInitalScene)
                {
                    GameSceneManager.Instance.StartingScene = startingDebugScene;
                }
                
            }
                if (managersList == null)
                {
                    managersList = new List<IManager>();
                    var allManagers = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                                 .OfType<IManager>()
                                 .Where(m => !(m is GameManager)&&(m!=null)); // excluir GameManager

                    managersList.AddRange(allManagers);
                    StartManager();
                }

            
        }
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnChangeScene;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnChangeScene;
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
            onPause?.Invoke(true);
        }
        public void UnPauseGame()
        {
            gameState = GameState.INGAME;
            onPause?.Invoke(false);
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
        private IEnumerator DelayedStartGame(Scene scene)
        {
            yield return new WaitForEndOfFrame();
            StartManager();
        }
        public void StartManager()
        {
            Debug.Log($"[{name}]:Iniciando...");
            managersList = managersList.Where(m => m != null).ToList();

            foreach (var manager in managersList.Where(m => m != null))
            {
                manager.StartManager();
            }
            LoadData();
        }

        public void OnStartGame()
        {
            Debug.Log($"[{name}]Empezando juego");
            //hay varios tipos de arranque de manager(unos dependen de otros) por defecto empiezan en normal

            foreach (var manager in managersList.FindAll(m => m.StartMode == IManager.GameStartMode.EARLY))
            {
                manager.OnStartGame();
            }
            foreach (var manager in managersList.FindAll(m => m.StartMode == IManager.GameStartMode.NORMAL))
            {
                manager.OnStartGame();
            }
            foreach (var manager in managersList.FindAll(m => m.StartMode == IManager.GameStartMode.LATE))
            {
                manager.OnStartGame();
            }

            //Logica de empezar el juego ya del gamemanager, que ocurre primero, de momento se empieza con la seleccion de cartas
            if (!(DebugGame && SkipPhase && SkipCardSelectionPhase))
            {
                CardManager.Instance.OnStartCardSelection();
            }
            else //asignar de forma random
            {
                CardManager.Instance.DebugStartCardSelection();
            }


        }
        public void OnDestroy()
        {
            managersList.Clear();
        }
    }
}