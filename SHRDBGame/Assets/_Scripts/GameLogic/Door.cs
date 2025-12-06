using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] protected Collider triggerInside;
    [SerializeField] protected Collider triggerOutside;

    [SerializeField]
    protected float openInsideDegrees;
    [SerializeField]
    protected float closedDegrees;
    [SerializeField]
    protected float openOutsideDegrees;
    [SerializeField] protected bool isLocked = false;

    public bool IsInteractable => isInteractable;
    private bool isInteractable = false;
    public bool isOpen = false;
    protected LTDescr currentTween;

    [Header("Audio")]
    [SerializeField] protected ASoundPlayer soundPlayer;
    [SerializeField] protected int openSoundIndex = 0;//se puede hacer enum pero vale
    [SerializeField] protected int closeSoundIndex = 1;
    void Start()
    {
        soundPlayer = GetComponent<ASoundPlayer>();
    }
    [ContextMenu("Poner Grados")]
    public void SetDegrees()
    {
        openInsideDegrees = closedDegrees - 90f;
        openOutsideDegrees = closedDegrees + 90f;
    }

    public string GetInteractionText()
    {
        if (!isInteractable) return "";
        if (isLocked) return "Bloqueada";
        return "Pulsa E";
    }
    public void UnLockDoor()
    {
        isLocked = false;
    }
    public void LockDoor()
    {
        isLocked = true;
        if(isOpen)
        {
            RotateDoor(FindAnyObjectByType<SimplePlayerController>().transform.position);
        }
    }
    public virtual void RotateDoor(Vector3 referencePos)
    {
        if (isLocked&&!isOpen)
        {
            soundPlayer.PlaySound(closeSoundIndex);
            return;
        }
        // Dirección 
        Vector3 dir = (referencePos - transform.position).normalized;

        // Dot para saber si está delante o detrás
        float dot = Vector3.Dot(transform.forward, dir);

        float targetY = closedDegrees;

        if (!isOpen)
        {
            // Abrir hacia dentro o hacia fuera
            bool opensOutside = dot > 0; // delante de la puerta

            targetY = opensOutside ? openOutsideDegrees : openInsideDegrees;

            // Activar/desactivar triggers
            triggerInside.enabled = opensOutside;   // si abre hacia fuera, interior queda activo
            triggerOutside.enabled = !opensOutside; // y el exterior se desactiva

            // 🔊 SONIDO DE ABRIR (NUEVO)
            if (soundPlayer != null)
                soundPlayer.PlaySound(openSoundIndex);
        }
        else
        {
            // Cerrar → activar ambos triggers otra vez
            targetY = closedDegrees;

            triggerInside.enabled = true;
            triggerOutside.enabled = true;

            // 🔊 SONIDO DE CERRAR (NUEVO)
            if (soundPlayer != null)
                soundPlayer.PlaySound(closeSoundIndex);
        }

        // Aplicar rotación
        // Vector3 e = transform.localEulerAngles;
        // e.y = targetY;
        // transform.localEulerAngles = e;
        AnimateDoor(targetY);

        isOpen = !isOpen;
    }

    public void Interact()
    {
        if (!isInteractable) return;
        isInteractable = false;

        Vector3 playerPos = FindAnyObjectByType<SimplePlayerController>().transform.position;

        RotateDoor(playerPos);
    }

    private void AnimateDoor(float targetY)
    {
        // Cancelar tween previo si existe
        if (currentTween != null)
            LeanTween.cancel(gameObject);

        isInteractable = false; // bloquear interacción durante la animación

        // Ángulo inicial (0..360)
        float startY = transform.localEulerAngles.y;

        // Delta angular mínimo (firma: positivo = giro en sentido 'positivo')
        float delta = Mathf.DeltaAngle(startY, targetY); // rango (-180,180]

        // Animamos desde 0 hasta delta, y aplicamos startY + valor para seguir la ruta más corta
        currentTween = LeanTween.value(gameObject, 0f, delta, 0.5f)
            .setEaseOutQuad()
            .setOnUpdate((float d) =>
            {
                float newY = startY + d;
                Vector3 e = transform.localEulerAngles;
                e.y = newY;
                transform.localEulerAngles = e;
            })
            .setOnComplete(() =>
            {
                // Asegurar que el ángulo final exactamente sea targetY (normalizado)
                Vector3 e = transform.localEulerAngles;
                e.y = targetY;
                transform.localEulerAngles = e;

                isInteractable = true;
                currentTween = null;
            }).setOnComplete(() =>
            {
                if (!isOpen)
                {
                    FindObjectOfType<CameraController>().Shake(.5f, 1f, 1);
                }

            });
    }

    public void SetInteractable(bool value)
    {
        isInteractable = value;
    }
    public Transform GetTransform()
    {
        return transform;
    }
}