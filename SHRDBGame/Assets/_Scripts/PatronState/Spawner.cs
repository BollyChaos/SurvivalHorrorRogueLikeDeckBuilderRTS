using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    //Falta meter que se destruyan los enemigos de la object pool, no se en que script ponerlo
    //*los enemigos no se destruyen en una object pool, de eso se trata el patron, de tocar lo menos posible la memoria
    //Falta tambien controlar cuantos enemigos deben existir en la escena
    [SerializeField] private Transform[] spawnPoints;
    int nVecinos = 5;
    public int NVecinos { get => nVecinos; set => nVecinos = value; }
    [SerializeField] private float timeBetweenSpawns = 1;
    public float TimeBetweenSpawns{get=>timeBetweenSpawns;set=>timeBetweenSpawns=value;}
    private float counter = 0f;
    public bool CanSpawnEnemies=false;

    [SerializeField] private GameObject enemyPrefab;
    public List<GameObject> vecinos;
    void Update()
    {

        if(!CanSpawnEnemies)return;
        if (counter >= timeBetweenSpawns)
        {
            counter = 0;

            CreateEnemy();


        }
        else
        {
            counter += Time.deltaTime;
        }



    }
    GameObject GetFirstDisabled()
    {
        foreach (var c in vecinos)
        {
            if (c != null && !c.gameObject.activeSelf)
                return c;
        }
        return null;
    }

    void CreateEnemy()
    {

        GameObject vecino = GetFirstDisabled();
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        if (vecino == null&&vecinos.Count<nVecinos)
        {

            GameObject enemy = Instantiate(enemyPrefab,transform);
            enemy.transform.position = randomSpawnPoint.position;
            enemy.SetActive(true);
            vecinos.Add(enemy);
        }
        else if(vecino!=null)
        {
            vecino.SetActive(true);
            vecino.transform.position = randomSpawnPoint.position;
        }

    }
public void StopEnemies()
    {
        CanSpawnEnemies=false;
        foreach(var vecino in vecinos)
        {
            vecino.gameObject.SetActive(false);
        }
    }

    // private IObjectPool<VecinoController> enemyPool;

    // private void Awake()
    // {//habria que poner un metodo para suscribirse on startgame no en el awake
    // //el spawner tiene que estar controlado por algun manager no puede ir por su cuenta
    //     enemyPool = new ObjectPool<VecinoController>(CreateEnemy, OnGet, OnRelease);
    // }

    // private void OnGet (VecinoController enemy)
    // {
    //     enemy.gameObject.SetActive(true);
    //     Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
    //     enemy.transform.position = randomSpawnPoint.position;
    // }
    // private void OnRelease(VecinoController enemy)
    // {
    //     enemy.gameObject.SetActive(false);
    // }
    // private VecinoController CreateEnemy()
    // {
    //     VecinoController enemy = Instantiate(enemyPrefab);
    //     enemy.SetPool(enemyPool);
    //     return enemy;
    // }
    // void Update()
    // {
    //     if (Time.time > timeSinceLastSpawn)
    //     //ver si el tamaño de la pool es menor que el numero de enemigos que deberia haber en juego, normal que se pete
    //     {
    //         //Spawnear enemigo y resetear tiempo
    //         enemyPool.Get();
    //         timeSinceLastSpawn = Time.time + timeBetweenSpawns;
    //     }

    // }
}
