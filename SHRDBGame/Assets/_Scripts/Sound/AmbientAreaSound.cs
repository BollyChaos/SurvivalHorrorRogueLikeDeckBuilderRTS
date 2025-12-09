using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientAreaSound : MonoBehaviour
{
    [SerializeField] private Collider triggerCollider;
    [SerializeField] private ASoundPlayer soundPlayer;
    [SerializeField] private int soundIndex = 0;

    private void Awake()
    {
        if (triggerCollider != null)
            triggerCollider.isTrigger = true;
    }

    private void OnEnable()
    {
        if (triggerCollider != null)
            PhysicsCallbacks.Register(triggerCollider, OnTriggerEnterCallback, OnTriggerExitCallback);
    }

    private void OnDisable()
    {
        if (triggerCollider != null)
            PhysicsCallbacks.Unregister(triggerCollider, OnTriggerEnterCallback, OnTriggerExitCallback);
    }

    private void OnTriggerEnterCallback(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (soundPlayer != null)
                soundPlayer.PlayLoop(soundIndex);
        }
    }

    private void OnTriggerExitCallback(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (soundPlayer != null)
                soundPlayer.StopLoop();
        }
    }
}