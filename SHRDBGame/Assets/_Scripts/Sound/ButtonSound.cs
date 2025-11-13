using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;
    private float cooldownSoundTime = 0.2f;//por sea caso que no se pete
    private float counter = 0f;
    private bool canPlaySound = true;
    public void PlayButtonSound()
    {
        if (!canPlaySound) return;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(clickSound);
        canPlaySound = false;
        counter = 0f;
    }
    void Update()
    {
        if (canPlaySound) return;
        if (counter < cooldownSoundTime)
        {
            counter += Time.deltaTime;
        }
        if (counter >= cooldownSoundTime)
        {
            canPlaySound = true;
        }
    }
}