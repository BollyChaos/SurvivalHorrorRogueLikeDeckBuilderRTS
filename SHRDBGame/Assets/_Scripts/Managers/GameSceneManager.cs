using Patterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Managers
{
    public class GameSceneManager : ASingleton<GameSceneManager>, IManager
    {
        public enum SceneIds { BOOTSTRAP, MAINMENUSCENE, GAMESCENE}
        public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;
        [Header("Scene to start")]
        [SerializeField] public SceneIds StartingScene=SceneIds.MAINMENUSCENE;

       

        public void StartManager()
        {
            Debug.Log($"[{name}]:Iniciando...");
            LoadMenuScene();
        }
        public void LoadMenuScene()
        {
            SceneManager.LoadScene((int)StartingScene, LoadSceneMode.Single);
            //LoadSceneAsyncID((int)StartingScene);
        }
        public void LoadSceneAsyncID(int id)
        {
            StartCoroutine(LoadSceneAsyncIDRoutine(id));
        }
        private IEnumerator LoadSceneAsyncIDRoutine(int id)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(id, LoadSceneMode.Single);
            while (!op.isDone)
            {
                //Se puede mostrar barra de carga por aqui
                yield return null;
            }
        }
        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneAsyncRoutine(sceneName));
        }

        private IEnumerator LoadSceneAsyncRoutine(string sceneName)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (!op.isDone)
            {
                //Se puede mostrar barra de carga por aqui
                yield return null;
            }
        }
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

        public void SaveData()
        {
            throw new System.NotImplementedException();
        }

        public void OnStartGame()
        {
        }
    }
}