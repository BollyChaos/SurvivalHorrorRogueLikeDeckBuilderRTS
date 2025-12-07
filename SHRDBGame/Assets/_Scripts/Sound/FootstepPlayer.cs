using UnityEngine;

public class FootstepPlayer : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] footstepClips;

    [Header("Paso")]
    public float stepDelay = 0.4f;
    public float boostPitch = 1.5f;

    [HideInInspector]
    public bool boostActive = false;

    private int currentStepIndex = 0;
    private float lastStepTime;
    private Rigidbody rb;
    private bool wasMoving;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (audioSource == null)
            Debug.LogWarning("FootstepPlayer: No hay AudioSource asignado.");
    }

    void Update()
    {
        bool isMoving = rb.velocity.magnitude > 0.1f;

        if (isMoving)
        {
            // Pitch normal o boost
            audioSource.pitch = boostActive ? boostPitch : 1f;

            if (Time.time - lastStepTime > stepDelay)
            {
                PlayNextFootstep();
                lastStepTime = Time.time;
            }
        }
        else
        {
            if (wasMoving)
            {
                currentStepIndex = 0;
                audioSource.Stop();
            }
        }

        wasMoving = isMoving;
    }

    void PlayNextFootstep()
    {
        if (footstepClips.Length == 0) return;

        audioSource.PlayOneShot(footstepClips[currentStepIndex]);

        currentStepIndex=(currentStepIndex+1)%footstepClips.Length;
       
    }
}