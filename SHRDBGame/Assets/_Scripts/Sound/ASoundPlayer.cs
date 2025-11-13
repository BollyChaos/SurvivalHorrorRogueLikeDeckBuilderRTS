using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class ASoundPlayer : MonoBehaviour
{
    //clase basica para reproducir cualquier tipo de sonido
    [SerializeField]
    List<AudioClip> audioClips;//puede ser uno solo
    [SerializeField]
    float pitchVariation = 0.1f;
    AudioSource audioSource;
    private int soundIndex = 0;
    void Start()
    {
        audioSource=GetComponent<AudioSource>();
    }
    public void PlaySound(int soundidx = 0)
    {

        soundIndex = CheckSoundIndex(soundidx);
        audioSource.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
        audioSource.PlayOneShot(audioClips[soundIndex]);
    }
    public void PlayRandomSound()
    {
        soundIndex = CheckSoundIndex(Random.Range(0, audioClips.Count));
        audioSource.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
        audioSource.PlayOneShot(audioClips[soundIndex]);
    }
    private int CheckSoundIndex(int idx)
    {
        Mathf.Clamp(idx, 0, audioClips.Count - 1);
        return soundIndex;
    }
}
