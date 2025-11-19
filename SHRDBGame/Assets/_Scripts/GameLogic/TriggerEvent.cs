using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
public class TriggerEvent : MonoBehaviour
{
    [SerializeField]public
    UnityEvent onTriggerEnterEvent;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[{name}] Activando evento");
            onTriggerEnterEvent?.Invoke();
            gameObject.SetActive(false);
        }
        
    }
   
}

