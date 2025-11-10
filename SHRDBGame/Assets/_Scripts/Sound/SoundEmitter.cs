using UnityEngine;
using UnityEngine.Events;

public class SoundEmitter : MonoBehaviour
{
    public UnityEvent<Vector3> OnSoundEmitted;

    [ContextMenu("Emitir sonido ahora")]
    public void EmitNoise()
    {
        OnSoundEmitted.Invoke(transform.position);
        Debug.Log("Noise emitted at position: " + transform.position);
    }

}
