using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimation : MonoBehaviour
{
    [Header("Curve Movement Settings")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private float curveAmplitude = 50f;
    private Vector3 initialScale;
    private Vector3 initialRotation;    
    private LTDescr scaleTweenId;
private LTDescr rotateTweenId ;

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
        scaleTweenId=LeanTween.scale(rectTransform.gameObject, new Vector3(scale, scale), duration)
                 .setEase(LeanTweenType.easeOutBack);
    }
    public void RotateXValue(RectTransform rectTransform, float degrees)
    {
        LeanTween.rotateX(rectTransform.gameObject, degrees, duration)
                 .setEase(LeanTweenType.easeInOutCubic);
        rectTransform.localRotation = Quaternion.Euler(0f, rectTransform.localEulerAngles.y, rectTransform.localEulerAngles.z);

    }
    public void ScaleAndRotateZValue(RectTransform rectTransform,float initScale, float scale, float degrees)
    {
        initialRotation = rectTransform.localEulerAngles;
        initialScale = new Vector3(initScale, initScale, initScale); 

        scaleTweenId = LeanTween.scale(rectTransform.gameObject, new Vector3(scale, scale), duration)
                 .setEase(LeanTweenType.easeOutBack).setLoopPingPong();
        rotateTweenId = LeanTween.rotateZ(rectTransform.gameObject, degrees, duration)
                 .setEase(LeanTweenType.easeInOutCubic).setLoopPingPong();
              
    }
    public void CancelAnimations(RectTransform rectTransform)
    {
        if (scaleTweenId != null)
        {
            LeanTween.cancel(scaleTweenId.id);
            scaleTweenId=null;
        }
        if (rotateTweenId != null)
        {
            LeanTween.cancel(rotateTweenId.id);
            rotateTweenId = null;
        }
        if (initialRotation != null)
        {
            rectTransform.localEulerAngles = initialRotation;
        }
        if (initialScale != null)
        {
            rectTransform.localScale = initialScale;
        }

    }
    
}
