using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCombat : MonoBehaviour
{
    public Stats stats;
    private bool godStatus = false;
    private bool unlimitedDamage = false;
    private bool healOnDamage = false;
    public bool HealOnDamage { get => healOnDamage; set => healOnDamage = value; }
    private bool reflectDamage = false;
    public bool ReflectDamage { get => reflectDamage; set => reflectDamage = value; }

    public UnityEvent<bool> OnChangeHealth;

    [Header("Audio")]
    [SerializeField] private ASoundPlayer deathAudioPlayer;
    [SerializeField] private int deathClipIndex = 0;

    [Header("Low Health Audio")]
    [SerializeField] private ASoundPlayer lowHealthAudioPlayer;
    [SerializeField] private int lowHealthClipIndex = 0;
    [SerializeField] private float lowHealthThreshold = 30f;

    private bool alreadyDied = false;
    private bool lowHealthSoundPlaying = false;

    private void Start()
    {
        stats.CurrentHealth = stats.MaxHealth;
        UIManager.Instance.SetPlayerHealthUI(stats.CurrentHealth / stats.MaxHealth);

        SettingsManager.Instance.onSettingsChange.AddListener(onSettingsChange);
        onSettingsChange();
    }

    private void onSettingsChange()
    {
        godStatus = SettingsManager.Instance.GetValue<bool>("Invincible");
        unlimitedDamage = SettingsManager.Instance.GetValue<bool>("UnlimitedDamage");
        stats.MaxHealth = SettingsManager.Instance.GetValue<float>("PlayerHealth");
    }

    public void TakeDamage(float amount)
    {
        if (godStatus || alreadyDied) return;

        if (healOnDamage)
        {
            Heal(amount);
            healOnDamage = false;
            return;
        }
        else if (reflectDamage)
        {
            reflectDamage = false;
            return;
        }

        stats.TakeDamage(amount);

        transform.parent.GetComponent<CameraController>().Shake(
            1.5f,
            (amount / stats.MaxHealth) * 6f,
            (amount / stats.MaxHealth) * 6f
        );

        OnChangeHealth.Invoke(true);

        CheckLowHealthSound();

        if (!stats.IsAlive())
        {
            alreadyDied = true;

            LevelManager.Instance.EndGame();

            if (deathAudioPlayer != null)
                deathAudioPlayer.PlaySound(deathClipIndex);

            if (lowHealthAudioPlayer != null)
                lowHealthAudioPlayer.StopLoop();
        }

        UIManager.Instance.SetPlayerHealthUI(stats.CurrentHealth / stats.MaxHealth);
    }

    public void Heal(float amount)
    {
        stats.Heal(amount);
        UIManager.Instance.SetPlayerHealthUI(stats.CurrentHealth / stats.MaxHealth);
        OnChangeHealth.Invoke(false);

        CheckLowHealthSound();
    }

    private void CheckLowHealthSound()
    {
        if (stats.CurrentHealth <= lowHealthThreshold)
        {
            if (!lowHealthSoundPlaying && lowHealthAudioPlayer != null)
            {
                lowHealthAudioPlayer.PlayLoop(lowHealthClipIndex);
                lowHealthSoundPlaying = true;
            }
        }
        else
        {
            if (lowHealthSoundPlaying && lowHealthAudioPlayer != null)
            {
                lowHealthAudioPlayer.StopLoop();
                lowHealthSoundPlaying = false;
            }
        }
    }
    public void ActivateTemporaryInvincibility(float duration)
    {
        StartCoroutine(TempInvincibility(duration));
    }

    private IEnumerator TempInvincibility(float duration)
    {
        godStatus = true;
        yield return new WaitForSeconds(duration);
        godStatus = false;
    }
}