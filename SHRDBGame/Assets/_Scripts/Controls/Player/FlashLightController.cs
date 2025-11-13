using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLightController : MonoBehaviour
{
    [SerializeField] private GameObject flashLight;
    [SerializeField] private float minBlinkInterval = 5f; // tiempo mínimo entre parpadeos
    [SerializeField] private float maxBlinkInterval = 10f; // tiempo máximo entre parpadeos
    [SerializeField] private float blinkDuration = 0.1f;   // duración del parpadeo

    private bool isFlashLightActive = true;
    private Coroutine flickerRoutine;

    void Start()
    {
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
    }

    private IEnumerator FlickerRoutine()//esta corrutina me da mucho miedo pero creo que esta controlada
    {
        while (true)
        {
            // Espera un tiempo aleatorio antes del siguiente parpadeo
            float waitTime = Random.Range(minBlinkInterval, maxBlinkInterval);
            yield return new WaitForSeconds(waitTime);

            // Solo parpadea si está encendida
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
