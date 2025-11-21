using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider))]
public class TriggerEvent : MonoBehaviour
{
    [SerializeField]
    public UnityEvent onTriggerEnterEvent;
    [SerializeField]
    string targetTag;
    public bool destroyOnTrigger=false;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Debug.Log($"[{name}] Activando evento");
            onTriggerEnterEvent?.Invoke();
            gameObject.SetActive(false);
            if(destroyOnTrigger)Destroy(gameObject);
        }

    }

}

