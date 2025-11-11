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
    [SerializeField] private float timeBetweenSpawns = 60f;
    private float timeSinceLastSpawn;

    [SerializeField] private EnemyController enemyPrefab;
    private IObjectPool<EnemyController> enemyPool;

    private void Awake()
    {//habria que poner un metodo para suscribirse on startgame no en el awake
    //el spawner tiene que estar controlado por algun manager no puede ir por su cuenta
        enemyPool = new ObjectPool<EnemyController>(CreateEnemy, OnGet, OnRelease);
    }

    private void OnGet (EnemyController enemy)
    {
        enemy.gameObject.SetActive(true);
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        enemy.transform.position = randomSpawnPoint.position;
    }
    private void OnRelease(EnemyController enemy)
    {
        enemy.gameObject.SetActive(false);
    }
    private EnemyController CreateEnemy()
    {
        EnemyController enemy = Instantiate(enemyPrefab);
        enemy.SetPool(enemyPool);
        return enemy;
    }
    void Update()
    {
        if (Time.time > timeSinceLastSpawn)
        //ver si el tamaño de la pool es menor que el numero de enemigos que deberia haber en juego, normal que se pete
        {
            //Spawnear enemigo y resetear tiempo
            enemyPool.Get();
            timeSinceLastSpawn = Time.time + timeBetweenSpawns;
        }
            
    }
}
