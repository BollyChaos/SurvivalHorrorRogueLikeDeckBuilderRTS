using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ASoundPlayer))]
public class FlashLightController : MonoBehaviour
{
    [SerializeField] private GameObject flashLight;
    [SerializeField] private float minBlinkInterval = 5f;
    [SerializeField] private float maxBlinkInterval = 10f;
    [SerializeField] private float blinkDuration = 0.1f;

    private bool isFlashLightActive = true;
    private Coroutine flickerRoutine;

    private ASoundPlayer soundPlayer;

    void Start()
    {
        soundPlayer = GetComponent<ASoundPlayer>();

        LookForInput();

        if (flashLight != null)
            flickerRoutine = StartCoroutine(FlickerRoutine());
    }

    public void LookForInput()
    {
        PlayerInput input = InputManager.Instance.Input;
        if (input != null)
        {
            input.actions["FlashLightButton"].started += ReadFlashLightInput;
        }
    }

    public void ReadFlashLightInput(InputAction.CallbackContext ctx)
    {
        isFlashLightActive = !isFlashLightActive;
        flashLight.SetActive(isFlashLightActive);

        if (soundPlayer != null)
        {
            if (isFlashLightActive)
                soundPlayer.PlaySound(0); // sonido de encendido
            else
                soundPlayer.PlaySound(1); // sonido de apagado
        }
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minBlinkInterval, maxBlinkInterval);
            yield return new WaitForSeconds(waitTime);

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
