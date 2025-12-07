using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyVisionTrigger : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;

    private void Awake()
    {
        if (enemyController == null)
            enemyController = GetComponentInParent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyController?.OnPlayerEnterVision(other.gameObject);

            if (enemyController is TioController tio)
            {
                tio.OnSoundHeard(other.transform.position);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            enemyController?.OnPlayerStayVision(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyController?.OnPlayerExitVision();
        }
    }
}