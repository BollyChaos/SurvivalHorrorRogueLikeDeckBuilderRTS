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
        [SerializeField] private List<AudioClip> audioClips;

        public IManager.GameStartMode StartMode => IManager.GameStartMode.NORMAL;

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
            PlayMusic(audioClips[0]);//voy a poner uno para que el recolector de basura no lo borre por no hacer nada
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
        }

        public void OnEnd()
        {
            SaveData();
            Debug.Log($"[{name} cerrando...]");
        }

        public void OnStartGame()
        {
        }
        private void OnDestroy()
        {
        }

    }
}