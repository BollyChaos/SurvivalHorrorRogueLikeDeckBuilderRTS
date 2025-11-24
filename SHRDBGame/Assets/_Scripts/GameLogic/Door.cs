using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField]
    float openInsideDegrees;
    [SerializeField]
    float closedDegrees;
    [SerializeField]
    float openOutsideDegrees;
    public bool IsInteractable =>isInteractable;
    private bool isInteractable=false;
bool isOpen = false; // Añádelo arriba como campo privado

    [ContextMenu("Poner Grados")]
    public void SetDegrees()
    {
        openInsideDegrees=closedDegrees-90f;
        openOutsideDegrees=closedDegrees+90f;   
    }

    public string GetInteractionText()
    {
        if(!isInteractable) return "";
        return "Pulsa E para abrir la puerta";
    }

    

public void Interact()
{
    if (!isInteractable) return;

    Vector3 playerPos = FindAnyObjectByType<SimplePlayerController>().transform.position;

    // Dirección desde la puerta hacia el jugador
    Vector3 dir = (playerPos - transform.position).normalized;

    // ¿Jugador delante o detrás? (según el forward de la puerta)
    float dot = Vector3.Dot(transform.forward, dir);

    float targetY = closedDegrees;

    if (!isOpen)
    {
        // Abrir hacia dentro o hacia fuera
        targetY = (dot > 0) ? openOutsideDegrees : openInsideDegrees;
    }
    else
    {
        // Cerrar
        targetY = closedDegrees;
    }

    // Aplicar rotación instantáneamente (puedes animarlo luego si quieres)
    Vector3 e = transform.localEulerAngles;
    e.y = targetY;
    transform.localEulerAngles = e;

    isOpen = !isOpen;
    isInteractable=false;
}


    public void SetInteractable(bool value)
    {
        isInteractable=value;
    }
    public Transform GetTransform()
    {
        return transform;
    }
}
