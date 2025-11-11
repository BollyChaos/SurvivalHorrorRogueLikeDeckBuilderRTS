using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Camera playerCamera;
    public Camera PlayerCamera { get => playerCamera; }
    
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;

    [SerializeField] float defaultAmplitude = 2f;
    [SerializeField] float defaultFrequency = 2f;

    void Awake()
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();

        // ahora el "noise" se obtiene de esta manera
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }
    [ContextMenu("Probar Shake")]
public void TestShake()
    {
        Shake(1f,defaultAmplitude,defaultFrequency);
    }
    public void Shake(float duration, float amplitude = -1f, float frequency = -1f)
    {
        Debug.Log("Me llaman");
        if (amplitude < 0) amplitude = defaultAmplitude;
        if (frequency < 0) frequency = defaultFrequency;

        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;
        shakeTimer = duration;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0)
            {
                noise.m_AmplitudeGain = 0f;
                noise.m_FrequencyGain = 0f;
            }
        }
    }
    void OnDestroy()
    {
          noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }

}
