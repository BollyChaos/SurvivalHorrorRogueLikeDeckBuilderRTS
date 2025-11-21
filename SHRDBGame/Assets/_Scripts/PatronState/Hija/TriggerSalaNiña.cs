using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSalaNiña : MonoBehaviour
{
    //atributo
    private EnemyManager enemyManager;
    // Start is called before the first frame update
    void Start()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyManager.PlayerinHijaRoom();
            //Debug.Log("Player entered the room, salon abierto set to true");
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyManager.PlayerOutHijaRoom();
            //Debug.Log("Player left the room, salon abierto set to false");
        }
    }
}
