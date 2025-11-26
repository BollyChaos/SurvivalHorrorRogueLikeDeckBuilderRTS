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
    [SerializeField] float blackOutTime = 6f;

    [Header("MoneyRainEvent")]
    [SerializeField]
    GameObject moneyPrefab;
    [SerializeField]
    int nMoneyDrops = 5;
    [SerializeField]
    float moneyTime = 15;

    [Header("HealthRainEvent")]
    [SerializeField]
    GameObject healthPrefab;
    [SerializeField]
    int nHealthDrops = 3;
    [SerializeField]
    float healthTime = 15;

    [SerializeField]
    bool canVarySize = false;
    [Header("SpawnCardEvent")]
    [SerializeField]
    GameObject cardDropPrefab;
    [SerializeField]
    float cardTime = 15;

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
            case GameEvent.NONE:
                break;
            case GameEvent.ENEMIESAGRO:
                UIManager.Instance.ShowGameEventForAWhile("Agresividad de los enemigos aumentada(no implementado)");
                break;
            case GameEvent.SPAWNMONSTERS:
                UIManager.Instance.ShowGameEventForAWhile("Van a aparecer enemigos en la sala(no implementado)");
                break;
            case GameEvent.GOTORANDOMROOM:
                UIManager.Instance.ShowGameEventForAWhile("Ve a la sala (no implementado) en x segundos o muere");
                break;
            case GameEvent.LIGHTSOUT:
                StartCoroutine(LightsEvent());
                break;
            case GameEvent.MONEYRAIN:
                MoneyRain();
                break;
            case GameEvent.HEALTHRAIN:
                HealthRain();
                break;
            case GameEvent.SPAWNCARD:
            SpawnCard();
            break;
        }
    }

    private IEnumerator LightsEvent()
    {
        List<Light> sceneLights = new List<Light>(FindObjectsOfType<Light>());
        UIManager.Instance.ShowGameEventForAWhile("Luces fuera");
        foreach (var light in sceneLights)
        {
            light.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(blackOutTime);
        foreach (var light in sceneLights)
        {
            if (light != null)
                light.gameObject.SetActive(true);
        }
    }

    private void MoneyRain()
    {


        InstantiateDropsInRandomRoom(moneyPrefab, nMoneyDrops, moneyTime);


    }
    private void HealthRain()
    {


        InstantiateDropsInRandomRoom(healthPrefab, nHealthDrops, healthTime);


    }
    private void SpawnCard()
    {
        InstantiateDropsInRandomRoom(cardDropPrefab,1,cardTime);
    }

    private void InstantiateDropsInRandomRoom(GameObject prefab, int nDrops, float timeToDestroy)
    {

        List<RoomTrigger> rooms = new List<RoomTrigger>(FindObjectsOfType<RoomTrigger>());
        int nRoom = Random.Range(0, rooms.Count - 1);

        switch (currentEvent)
        {
            case GameEvent.MONEYRAIN:
                Debug.Log("Va a aparecer dinero en la sala " + rooms[nRoom].roomName + " durante " + timeToDestroy);
                UIManager.Instance.ShowGameEventForAWhile("Va a aparecer salud en la sala: " + rooms[nRoom].roomName + " durante " + timeToDestroy);
                break;
            case GameEvent.HEALTHRAIN:
                Debug.Log("Va a aparecer dinero en la sala " + rooms[nRoom].roomName + " durante " + timeToDestroy);
                UIManager.Instance.ShowGameEventForAWhile("Va a aparecer dinero en la sala: " + rooms[nRoom].roomName + " durante " + timeToDestroy);

                break;
            case GameEvent.SPAWNCARD:
                 Debug.Log("Va a aparecer una carta en la sala " + rooms[nRoom].roomName + " durante " + timeToDestroy);
                UIManager.Instance.ShowGameEventForAWhile("Va a aparecer una carta en la sala: " + rooms[nRoom].roomName + " durante " + timeToDestroy);
                break;
        }

        for (int i = 0; i < nDrops; i++)
        {
            Vector3 spawnPos = GetRandomPointInBox(rooms[nRoom].GetComponent<BoxCollider>());
            spawnPos += Vector3.up * 8;
            GameObject prefabInstantiated = Instantiate(prefab);
            prefabInstantiated.transform.localPosition = spawnPos;
            Destroy(prefabInstantiated, timeToDestroy);
        }
    }
    Vector3 GetRandomPointInBox(BoxCollider box)
    {
        Vector3 size = box.size;
        Vector3 center = box.center;

        // punto aleatorio dentro del volumen del collider (en espacio local)
        Vector3 randomLocalPos = new Vector3(
            Random.Range(-size.x * 0.5f, size.x * 0.5f),
            Random.Range(-size.y * 0.5f, size.y * 0.5f),
            Random.Range(-size.z * 0.5f, size.z * 0.5f)
        );

        // convertimos de espacio local del collider a mundo
        return box.transform.TransformPoint(center + randomLocalPos);
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
