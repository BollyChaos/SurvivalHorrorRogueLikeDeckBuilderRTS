using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAnimation : MonoBehaviour
{
    [Header("Floating Settings")]
    public float distance = 0.2f;
    public float time = 1.5f;

    private LTDescr _floatTween;
    public Vector3 _startLocalPos;

    private void Awake()
    {
        _startLocalPos = transform.localPosition;
    }

    private void OnEnable()
    {
        StartFloating();
    }

    private void OnDisable()
    {
        StopFloating();
    }

    /// <summary>
    /// Inicia la animación de flotar.
    /// </summary>
    public void StartFloating()
    {
        // si ya está animando, no lo replicamos
        if (_floatTween != null)
            return;

        _floatTween = LeanTween.moveLocalY(
            gameObject,
            _startLocalPos.y + distance,
            time
        )
        .setEaseInOutSine()
        .setLoopPingPong();
    }

    /// <summary>
    /// Detiene la animación completamente.
    /// </summary>
    public void StopFloating()
    {
        if (_floatTween != null)
        {
            LeanTween.cancel(gameObject);
            _floatTween = null;
            transform.localPosition = _startLocalPos;
        }
    }

    /// <summary>
    /// Reinicia la animación (stop + start).
    /// </summary>
    public void RestartFloating()
    {
        StopFloating();
        StartFloating();
    }
}
