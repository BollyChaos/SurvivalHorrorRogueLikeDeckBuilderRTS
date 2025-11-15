using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisionTrigger : MonoBehaviour
{
    [SerializeField]private EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            enemyController?.OnPlayerEnterVision(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            enemyController?.OnPlayerStayVision(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            enemyController?.OnPlayerExitVision();
    }
}

