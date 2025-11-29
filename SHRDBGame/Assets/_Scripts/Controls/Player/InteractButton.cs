using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class InteractButton : MonoBehaviour
{
    void OnEnable()
    {
        // if (!GameObject.Find("PlayerPrefab"))
        // {
        //     Debug.LogError("Hola");
        // }
        Interactor interactor = GameObject.Find("PlayerPrefab")?.transform.GetComponentInChildren<Interactor>(true);

        if (interactor == null)
        {
            return;
        }

        GetComponent<Button>().onClick.AddListener(interactor.ButtonInteract);
    }
    void OnDisable()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
    }
}
