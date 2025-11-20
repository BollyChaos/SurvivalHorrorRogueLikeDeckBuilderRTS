using System.Collections;
using System.Collections.Generic;
using Managers;
using Patterns.Singleton;
using Unity.VisualScripting;
using UnityEngine;

public class GameEventsManager : ASingleton<GameEventsManager>, IManager
{
    public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;
    public enum GameEvent { NONE, LIGHTSOUT, ENEMIESAGRO, SPAWNMONSTERS, GOTORANDOMROOM, MONEYRAIN, HEALTHRAIN, SPAWNCARD, }
    [Header("Events")]

    [SerializeField]
    public GameEvent currentEvent = GameEvent.NONE;
    [SerializeField] public float TimeBetweenEvents = 20f;
    [SerializeField] public float randomRange = 5f;
    private bool CanDoEvents = false;
    float timeToWait;
    private float counter = 0f;
    private float[] eventProbs;//de momento van a ser sucesos equiprobables
    private float decayFactor = .6f;
    [Header("LightsOutEvent")]
    [SerializeField] float blackOutTime=6f;
    [Header("Debug")]
    [SerializeField]
    bool debug = true;
    [SerializeField, ShowIf("debug")]
    private GameEvent fixedTimeEvent;
    #region MANAGERLOGIC
    void Update()
    {
        if (!CanDoEvents) return;
        if (counter >= timeToWait)
        {
            counter = 0f;
            timeToWait = UnityEngine.Random.Range(TimeBetweenEvents - randomRange, TimeBetweenEvents + randomRange);

            if (!debug)
            {
                currentEvent = (GameEvent)DynamicProbability.GetRandomIndexArgs(eventProbs, decayFactor);

            }
            else
            {
                currentEvent = fixedTimeEvent;
            }
            ThrowEvent();
            Debug.Log($"[{name}]VA A OCURRIR EL EVENTO {currentEvent}");
            Debug.Log($"[{name}]EVENTO EN {timeToWait} SEGUNDOS");

        }
        else counter += Time.deltaTime;

    }
    private void ThrowEvent()
    {
        switch (currentEvent)
        {
            case GameEvent.LIGHTSOUT:
           StartCoroutine( LightsEvent());
            break;
        }
    }
    private IEnumerator LightsEvent()
    {
        List<Light> sceneLights=new List<Light>(FindObjectsOfType<Light>());
        foreach(var light in sceneLights)
        {
            light.gameObject.SetActive(false);
        }
        
        yield return new WaitForSeconds(blackOutTime);
         foreach(var light in sceneLights)
        {
            light.gameObject.SetActive(true);
        }
    }
    public void LoadData()
    {
    }

    public void OnEnd()
    {
        Debug.Log($"[{name} cerrando...]");
    }

    public void OnEndGame()
    {
        StopAllCoroutines();//por sea caso que no se ralle el manager
        CanDoEvents = false;
    }

    public void OnStartGame()
    {
        Debug.Log($"[{name}]:Empezando juego");
        LevelManager.Instance.onNightStateChanged.AddListener(OnNightStateChanged);

    }
    public void OnNightStateChanged(bool isNight)

    {
        if (isNight)
        {
            timeToWait = UnityEngine.Random.Range(TimeBetweenEvents - randomRange, TimeBetweenEvents + randomRange);
            Debug.Log($"[{name}]EVENTO EN {timeToWait} SEGUNDOS");
            CanDoEvents = true;
        }
        else
        {
            CanDoEvents = false;
        }
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
