using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SlidingDoorLeft : MonoBehaviour, IInteractable
{
    [Header("NavMesh Obstacles")]
    [SerializeField] private NavMeshObstacle[] obstacle;

    [Header("Movement")]
    [SerializeField] private Vector3 closedPos;
    [SerializeField] private Vector3 openOffset = new Vector3(-1.5f, 0, 0); // SIEMPRE IZQUIERDA
    [SerializeField] private float moveTime = 0.5f;

    [Header("State")]
    [SerializeField] private bool isLocked = false;
    private bool isInteractable = true;
    public bool IsInteractable => isInteractable;
    public bool isOpen = false;

    private LTDescr currentTween;

    [Header("Audio")]
    [SerializeField] private ASoundPlayer soundPlayer;
    [SerializeField] private int openSoundIndex = 0;
    [SerializeField] private int closeSoundIndex = 1;

    void Start()
    {
        if (soundPlayer == null)
            soundPlayer = GetComponent<ASoundPlayer>();

        closedPos = transform.localPosition;
    }

    public string GetInteractionText()
    {
        if (!isInteractable) return "";
        if (isLocked) return "Bloqueada";
        return "???";
    }

    public void UnLockDoor() => isLocked = false;

    public void LockDoor()
    {
        isLocked = true;

        // Si está abierta, forzamos que se cierre ahora
        if (isOpen)
            SlideDoor();
    }

    public void Interact()
    {
        if (!isInteractable) return;
        SlideDoor();
    }

[ContextMenu("Mover Puerta")]
    public void SlideDoor()
    {
        if (isLocked && !isOpen)
        {
            soundPlayer?.PlaySound(closeSoundIndex);
            return;
        }

        Vector3 targetPos = isOpen
            ? closedPos
            : closedPos + openOffset;
            Debug.Log("Moviendose puerta a " + targetPos);

        // Sonidos
        if (!isOpen)
            soundPlayer?.PlaySound(openSoundIndex);
        else
            soundPlayer?.PlaySound(closeSoundIndex);

        AnimateDoor(targetPos);

        isOpen = !isOpen;

        // Actualizar carving del NavMesh
        if (obstacle != null)
        {
            foreach (var obs in obstacle)
                obs.carving = !isOpen;
        }
    }

    private void AnimateDoor(Vector3 targetLocalPos)
    {
        if (currentTween != null)
            LeanTween.cancel(gameObject);

        isInteractable = false;

        Vector3 start = transform.localPosition;

        currentTween = LeanTween.value(gameObject, 0f, 1f, moveTime)
            .setEaseOutQuad()
            .setOnUpdate((float t) =>
            {
                transform.localPosition = Vector3.Lerp(start, targetLocalPos, t);
            })
            .setOnComplete(() =>
            {
                transform.localPosition = targetLocalPos;
                isInteractable = true;
                currentTween = null;

                // Agitar cámara solo al cerrar
                if (!isOpen)
                    FindObjectOfType<CameraController>().Shake(.5f, 1f, 1);
            });
    }

    public void SetInteractable(bool value){isInteractable = value;}

    public Transform GetTransform() => transform;
}
