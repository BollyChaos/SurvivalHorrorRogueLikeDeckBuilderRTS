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

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(int soundIdx = 0)
    {
        soundIndex = CheckSoundIndex(soundIdx);
        audioSource.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
        audioSource.PlayOneShot(audioClips[soundIndex]);
    }

    public void PlayRandomSound()
    {
        soundIndex = CheckSoundIndex(Random.Range(0, audioClips.Count));
        audioSource.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
        audioSource.PlayOneShot(audioClips[soundIndex]);
    }

    public void AssignClips(List<AudioClip> clips)
    {
        audioClips = clips;
    }

    private int CheckSoundIndex(int idx)
    {
        soundIndex = Mathf.Clamp(idx, 0, audioClips.Count - 1);
        return soundIndex;
    }
}
