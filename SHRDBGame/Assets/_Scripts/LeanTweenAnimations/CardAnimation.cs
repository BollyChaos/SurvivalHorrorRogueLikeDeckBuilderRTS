using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAnimation : MonoBehaviour
{
    [Header("Curve Movement Settings")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private float curveAmplitude = 50f;
    [Header("Default transform"), Tooltip("Transform returns to these values after completing animations")]
    [SerializeField] Vector3 initialPosition;

    [SerializeField] Vector3 initialScale;
    [SerializeField] Vector3 initialRotation;
    private LTDescr moveTweenId;
    private LTDescr scaleTweenId;
    private LTDescr rotateTweenId;

    private LTDescr scaleTweenIdDisplayAnim;
    private LTDescr rotateTweenIdDisplayAnim;
    //Constructor de las animaciones
    public void InitTransform(Vector3 initPos, Vector3 initScale, Vector3 initRotation)
    {
        initialPosition = initPos;
        initialScale = initScale;
        initialRotation = initRotation;
    }
    public void ApplyScale()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = initialScale;
    }

    public void ResetTransform()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.position = initialPosition;//IMPORTANTE, PARA LA POSICION SE TRABAJA CON COORDENADAS DEL MUNDO,NO LAS LOCALES
        ApplyScale();
        rectTransform.localEulerAngles = initialRotation;

    }
    //Solo mover moveTweenId

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
        moveTweenId =

        LeanTween.rotateX(rectTransform.gameObject, 0, duration / 3)
                 .setEase(LeanTweenType.easeInOutCubic).setOnComplete(() =>
                 {
                     LeanTween.move(rectTransform.gameObject, path, duration)
                                       .setEase(LeanTweenType.easeInOutSine).setOnComplete(() =>
                                       {
                                           moveTweenId = null;
                                           ResetTransform();
                                       });
                 });
    }
    public void DisplayAnimation(RectTransform rectTransform, float initScale, float scale, float degrees)
    {
        ScaleAndRotateZValue(rectTransform, initScale, scale, degrees);
    }

    public void Scale(RectTransform rectTransform, float scale)
    {
        scaleTweenId = LeanTween.scale(rectTransform.gameObject, new Vector3(scale, scale), duration)
                 .setEase(LeanTweenType.easeOutBack);
    }
    private void RotateXValue(RectTransform rectTransform, float degrees)
    {
        LeanTween.rotateX(rectTransform.gameObject, degrees, duration / 3)
                 .setEase(LeanTweenType.easeInOutCubic).setOnComplete(() =>
                 {
                     moveTweenId = null;
                     ResetTransform();
                 });


    }
    public void ScaleAndRotateZValue(RectTransform rectTransform, float initScale, float scale, float degrees)
    {
      

        scaleTweenIdDisplayAnim = LeanTween.scale(rectTransform.gameObject, new Vector3(scale, scale), duration)
                 .setEase(LeanTweenType.easeOutBack).setLoopPingPong();
        rotateTweenIdDisplayAnim = LeanTween.rotateZ(rectTransform.gameObject, degrees, duration)
                 .setEase(LeanTweenType.easeInOutCubic).setLoopPingPong();

    }
    public void CancelDisplayAnimations(RectTransform rectTransform)
    {
        if (moveTweenId != null) { //Debug.Log("No se puede interrumpir lo siento"); 
        return; }//si todavia se esta moviendo la carta hacia el HUD no interrumpir
        if (scaleTweenIdDisplayAnim != null)
        {
            LeanTween.cancel(scaleTweenIdDisplayAnim.id);
            scaleTweenIdDisplayAnim = null;
        }
        if (rotateTweenIdDisplayAnim != null)
        {
            LeanTween.cancel(rotateTweenIdDisplayAnim.id);
            rotateTweenIdDisplayAnim = null;
        }
        LeanTween.cancel(gameObject);
        ResetTransform();
    }
    public void CancelAnimations(RectTransform rectTransform)
    {
        if (scaleTweenId != null)
        {
            LeanTween.cancel(scaleTweenId.id);
            scaleTweenId = null;
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
        if (moveTweenId != null)
        {
            LeanTween.cancel(moveTweenId.id);
            moveTweenId = null;
        }
        if (scaleTweenIdDisplayAnim != null)
        {
            LeanTween.cancel(scaleTweenIdDisplayAnim.id);
            scaleTweenIdDisplayAnim = null;
        }
        if (rotateTweenIdDisplayAnim != null)
        {
            LeanTween.cancel(rotateTweenIdDisplayAnim.id);
            rotateTweenIdDisplayAnim = null;
        }

    }

}
