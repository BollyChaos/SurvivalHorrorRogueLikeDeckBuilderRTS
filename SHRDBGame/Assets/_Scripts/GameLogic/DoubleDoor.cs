using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoor : Door
{
    LTDescr currentLeftTween;
    [Header("Double Door Parts")]
    [SerializeField] private GameObject rightDoor;
    [SerializeField] private GameObject leftDoor;

    [SerializeField] float openInsideDegreesLeft;
    [SerializeField] float closedDegreesLeft;
    [SerializeField] float openOutsideDegreesLeft;

    public override void RotateDoor(Vector3 referencePos)
    {
        if (isLocked)
        {
            soundPlayer.PlaySound(closeSoundIndex);
        return;
        }
        // Dirección jugador → puerta
        Vector3 dir = (referencePos - transform.position).normalized;

        float dot = Vector3.Dot(transform.forward, dir);

        // Ángulos por puerta
        float targetYLeft = isOpen ? closedDegreesLeft : (dot > 0 ? openOutsideDegreesLeft : openInsideDegreesLeft);

        float targetYRight = isOpen ? closedDegrees : (dot > 0 ? openOutsideDegrees : openInsideDegrees);

        // // Activar/desactivar triggers
        // triggerInside.enabled = !isOpen && dot > 0;   // ejemplo, se ajusta según lógica
        // triggerOutside.enabled = !isOpen && dot <= 0;

        // Animar puerta izquierda
        if (currentLeftTween != null) LeanTween.cancel(currentLeftTween.id);

        float startYLeft = leftDoor.transform.localEulerAngles.y;
        float deltaLeft = Mathf.DeltaAngle(startYLeft, targetYLeft);
        // Debug.LogWarning("Moviendo puerta izquierda de " + startYLeft + " a " + targetYLeft + " delta: " + deltaLeft);

        currentLeftTween = LeanTween.value(leftDoor, 0f, deltaLeft, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnUpdate((float d) =>
            {
                Vector3 e = leftDoor.transform.localEulerAngles;
                e.y = startYLeft + d;
                leftDoor.transform.localEulerAngles = e;
            }).setOnComplete(() =>
            {
                Debug.LogWarning("Animación puerta izquierda completa. Ángulo final: " + leftDoor.transform.localEulerAngles.y);
            });


        // Animar puerta derecha

        if (currentTween != null) LeanTween.cancel(currentTween.id);

        float startYRight = rightDoor.transform.localEulerAngles.y;
        float deltaRight = Mathf.DeltaAngle(startYRight, targetYRight);

        currentTween = LeanTween.value(rightDoor, 0f, deltaRight, 0.5f)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnUpdate((float d) =>
            {
                Vector3 e = rightDoor.transform.localEulerAngles;
                e.y = startYRight + d;
                rightDoor.transform.localEulerAngles = e;
            });


        isOpen = !isOpen;

        // Sonido
        if (soundPlayer != null)
            soundPlayer.PlaySound(isOpen ? openSoundIndex : closeSoundIndex);
    }

}
