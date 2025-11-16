using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [Header("Interacción")]
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] public bool isInteracting = false;
    private IInteractable currentTarget;

    // Lista de interactuables dentro del rango
    private readonly List<IInteractable> interactablesInRange = new();
    void Start()
    {
       
        LookForInput();
    }
    [ContextMenu("DecirInteractuables")]
    public void SayInteractables()
    {
        Debug.Log($"Interactuables en rango: {interactablesInRange.Count}");
    }
    public void LookForInput()
    {
        Debug.Log("BuscandoInputManager");
        PlayerInput input = InputManager.Instance.Input;
        if (input != null)
        {
            input.actions["Interact"].started += OnInteract;
            input.actions["Interact"].performed += OnInteract;
            input.actions["Interact"].canceled += OnInteract;

            Debug.Log("InputManager encontrado");
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        isInteracting = context.ReadValue<float>() > 0;


    }

    void Update()
    {
        // Detectar el interactuable más cercano dentro del rango
        if (interactablesInRange.Count > 0)
        {


            if (isInteracting)
            {
                currentTarget = GetClosestInteractable();
                if (currentTarget != null)
                    currentTarget.Interact();
            }
        }
        else
        {
            currentTarget = null;
        }
    }

    private IInteractable GetClosestInteractable()
    {
        float minDist = float.MaxValue;
        IInteractable closest = null;

        foreach (var i in interactablesInRange)
        {
            if (i == null) continue;

            float dist = Vector3.Distance(transform.position, i.GetTransform().position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = i;
            }
        }

        return closest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & interactableMask) != 0)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable != null && !interactablesInRange.Contains(interactable))
            {
                interactable.SetInteractable(true);
                interactablesInRange.Add(interactable);
            }
            currentTarget = GetClosestInteractable();
            if (currentTarget != null)
            {
                UIManager.Instance.SetInteractionText(currentTarget.GetInteractionText());
                if (currentTarget.GetTransform().tag.Equals("Purchaseable"))
                {
                    UIManager.Instance.ShowMoney(GetComponent<Economy>().Coins);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            interactable.SetInteractable(false);
            interactablesInRange.Remove(interactable);
            if (interactablesInRange.Count == 0)
            {
                UIManager.Instance.HideInteractionText();
                if (interactable.GetTransform().tag.Equals("Purchaseable"))
                {
                    //Debug.Log("ADios");
                    UIManager.Instance.HideMoney();
                }

            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

}
public interface IInteractable
{
    bool IsInteractable { get; }
    void Interact();
    void SetInteractable(bool value);
    Transform GetTransform();
    public string GetInteractionText();
}
