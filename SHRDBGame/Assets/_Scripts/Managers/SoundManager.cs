using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Patterns.Singleton;
namespace Managers
{
    public class SoundManager : ASingleton<SoundManager>, IManager
    {
        private enum SoundTrack { MENU, INTRO, DAY, NIGHT,DEATH }
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private List<AudioClip> audioClips;

        public IManager.GameStartMode StartMode => IManager.GameStartMode.LATE;

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
            PlayMusic(audioClips[(int)SoundTrack.MENU]);//voy a poner uno para que el recolector de basura no lo borre por no hacer nada
            LoadData();
        }

        public void LoadData()
        {
            GetComponent<SoundSettingsApplier>().Init();
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
            PlayMusic(audioClips[1]);
            LevelManager.Instance.onNightStateChanged.AddListener(OnNightChange);
        }
        public void OnPlayerDeath()
        {
            PlayMusic(audioClips[(int)SoundTrack.DEATH]);//voy a poner uno para que el recolector de basura no lo borre por no hacer nada
            
        }
        public void OnNightChange(bool isNight)
        {
            
                if (isNight)
                {
                    PlayMusic(audioClips[(int)SoundTrack.NIGHT]);
                }
                else
                {
                    PlayMusic(audioClips[(int)SoundTrack.DAY]);
                }
            
        }   
        private void OnDestroy()
        {
        }

    }
}