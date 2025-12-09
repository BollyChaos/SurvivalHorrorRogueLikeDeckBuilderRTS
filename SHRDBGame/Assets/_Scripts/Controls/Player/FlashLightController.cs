using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLightController : MonoBehaviour
{
    [SerializeField] private GameObject flashLight;

    [SerializeField] private float minBlinkInterval = 5f;
    [SerializeField] private float maxBlinkInterval = 10f;
    [SerializeField] private float blinkDuration = 0.1f;

    [Header("Audio")]
    [SerializeField] private ASoundPlayer soundPlayer;

    private bool isFlashLightActive = true;
    private Coroutine flickerRoutine;

    void Start()
    {
        if (flashLight != null)
            flickerRoutine = StartCoroutine(FlickerRoutine());

        LookForInput();
    }

    public void LookForInput()
    {
        PlayerInput input = InputManager.Instance.Input;
        if (input != null)
        {
            input.actions["FlashLightButton"].started += ReadFlashLightInput;
        }
    }
public void ReadFlashLightInput()
    {
         isFlashLightActive = !isFlashLightActive;
        flashLight.SetActive(isFlashLightActive);

        if (soundPlayer != null)
        {
            if (isFlashLightActive)
                soundPlayer.PlaySound(0);
            else
                soundPlayer.PlaySound(1);
        }
    }
    public void ReadFlashLightInput(InputAction.CallbackContext ctx)
    {
       ReadFlashLightInput();
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minBlinkInterval, maxBlinkInterval);
            yield return new WaitForSeconds(waitTime);
            GameManager.Instance.SetValue<float>("FlashLightSeconds", GameManager.Instance.GetValue<float>("FlashLightSeconds") + waitTime);
            if (isFlashLightActive && flashLight.activeSelf)
            {
                flashLight.SetActive(false);
                yield return new WaitForSeconds(blinkDuration);
                flashLight.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        PlayerInput input = InputManager.Instance.Input;
        if (input != null)
        {
            input.actions["FlashLightButton"].started -= ReadFlashLightInput;
        }

        if (flickerRoutine != null)
            StopCoroutine(flickerRoutine);
    }
}