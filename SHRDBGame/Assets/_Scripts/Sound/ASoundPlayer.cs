using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ASoundPlayer : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips;
    [SerializeField] private float pitchVariation = 0.1f;

    private AudioSource audioSource;
    private int soundIndex = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void PlaySound(int soundidx = 0)
    {
        if (audioClips == null || audioClips.Count == 0) return;

        soundIndex = Mathf.Clamp(soundidx, 0, audioClips.Count - 1);
        audioSource.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
        audioSource.PlayOneShot(audioClips[soundIndex]);
    }

    public void PlayRandomSound()
    {
        if (audioClips == null || audioClips.Count == 0) return;

        soundIndex = Random.Range(0, audioClips.Count);
        audioSource.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
        audioSource.PlayOneShot(audioClips[soundIndex]);
    }
    public void StopSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}