using System.Collections;
using System.Collections.Generic;
using Managers;
using Patterns.Singleton;
using Unity.VisualScripting;
using UnityEngine;

public class GameEventsManager : ASingleton<GameEventsManager>,IManager
{
    public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;
    public enum GameEvent { NONE, LIGHTSOUT, ENEMIESAGRO, SPAWNMONSTERS, GOTORANDOMROOM }
    [SerializeField]
    public GameEvent currentEvent = GameEvent.NONE;
    [SerializeField] public float TimeBetweenEvents = 20f;
    [SerializeField] public float randomRange = 5f;
    private bool CanDoEvents = false;
    float timeToWait;
    private float counter = 0f;
    private float[] eventProbs;//de momento van a ser sucesos equiprobables
    private float decayFactor = .6f;
    #region MANAGERLOGIC
    void Update()
    {
        if (!CanDoEvents) return;
        if (counter >= timeToWait)
        {
            counter = 0f;
            timeToWait = UnityEngine.Random.Range(TimeBetweenEvents - randomRange, TimeBetweenEvents + randomRange);
            currentEvent = (GameEvent)DynamicProbability.GetRandomIndexArgs(eventProbs, decayFactor);
            Debug.Log($"[{name}]VA A OCURRIR EL EVENTO {currentEvent}");
            Debug.Log($"[{name}]EVENTO EN {timeToWait} SEGUNDOS");

        }
        else counter += Time.deltaTime;
    
    }
    public void LoadData()
    {
    }

    public void OnEnd()
    {
        throw new System.NotImplementedException();
    }

    public void OnEndGame()
    {
        StopAllCoroutines();//por sea caso que no se ralle el manager
    }

    public void OnStartGame()
    {
        Debug.Log($"[{name}]:Empezando juego");
    }

    public void SaveData()
    {
        throw new System.NotImplementedException();
    }

    public void StartManager()
    {
        Debug.Log($"[{name}]:Iniciando...");
        eventProbs = new float[System.Enum.GetValues(typeof(GameEvent)).Length];//crear el array;
        for (int i = 0; i < eventProbs.Length; i++)
        {
            eventProbs[i] = 1f;
        }
        timeToWait = UnityEngine.Random.Range(TimeBetweenEvents - randomRange, TimeBetweenEvents + randomRange);
        Debug.Log($"[{name}]EVENTO EN {timeToWait} SEGUNDOS");
        CanDoEvents = true;
    }
    #endregion
    // IEnumerator CreateEvents()
    // {
    //     while (true)
    //     {
    //         float timeToWait = UnityEngine.Random.Range(TimeBetweenEvents - randomRange, TimeBetweenEvents + randomRange);
    //         Debug.Log($"[{name}]EVENTO EN {timeToWait} SEGUNDOS");
    //         yield return new WaitForSeconds(timeToWait);
    //         currentEvent = (GameEvent)DynamicProbability.GetRandomIndexArgs(eventProbs,decayFactor);
    //         Debug.Log($"[{name}]VA A OCURRIR EL EVENTO {currentEvent}");
    //     }

    // }
    void OnDestroy()
    {
        StopAllCoroutines();
    }

}
