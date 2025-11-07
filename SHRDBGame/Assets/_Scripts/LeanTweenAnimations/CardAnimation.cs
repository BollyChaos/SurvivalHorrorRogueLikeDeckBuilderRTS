using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimation : MonoBehaviour
{
    [Header("Curve Movement Settings")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private float curveAmplitude = 50f;
    public void MoveToCurve(RectTransform rectTransform, Vector3 targetPosition)
    {
        // Posición inicial


        Vector3 startPos = rectTransform.position;

        // Creamos puntos intermedios para la curva
        Vector3 midPoint1 = startPos + new Vector3(UnityEngine.Random.Range(-curveAmplitude, curveAmplitude),
                                                   UnityEngine.Random.Range(-curveAmplitude, curveAmplitude), 0f);
        Vector3 midPoint2 = startPos + new Vector3(UnityEngine.Random.Range(-curveAmplitude, curveAmplitude),
                                                   UnityEngine.Random.Range(-curveAmplitude, curveAmplitude), 0f);

        // Creamos el path
        Vector3[] path = new Vector3[] { startPos, midPoint1, midPoint2, targetPosition };

        // Movemos con la curva generada
        LeanTween.move(rectTransform.gameObject, path, duration)
                 .setEase(LeanTweenType.easeInOutSine);
    }
    public void Scale(RectTransform rectTransform, float scale)
    {
        LeanTween.scale(rectTransform.gameObject, new Vector3(scale, scale), duration)
                 .setEase(LeanTweenType.easeOutBack);
    }
    public void RotateXValue(RectTransform rectTransform, float degrees)
    {
        LeanTween.rotateX(rectTransform.gameObject, degrees, duration)
                 .setEase(LeanTweenType.easeInOutCubic);
        rectTransform.localRotation = Quaternion.Euler(0f, rectTransform.localEulerAngles.y, rectTransform.localEulerAngles.z);

    }
    
}
