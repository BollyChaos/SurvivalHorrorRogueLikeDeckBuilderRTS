using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns.Singleton;
namespace Managers
{
    public class SoundManager : ASingleton<SoundManager>, IManager
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        public void PlaySFX(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void StartManager()
        {
            Debug.Log($"[{name}]:Iniciando...");
            LoadData();
        }

        public void LoadData()
        {

        }

        public void SaveData()
        {

        }

        public void OnEndGame()
        {
            OnEnd();
            Destroy(gameObject);
        }

        public void OnEnd()
        {
            SaveData();
        }

        public void OnStartGame()
        {
        }
    }
}