using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCardAction : MonoBehaviour, ICardAction
{
    private enum TypeOfGun { Gun, Shotgun }

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] public Transform playerTransform;
    [SerializeField] private TypeOfGun typeOfGun;

    [Header("Gun Settings")]
    [SerializeField] float spreadAngle = 60f;
    [SerializeField] int bulletsCount = 4;
    //Lo voy a desacoplar

    // [Header("Audio")]
    // [SerializeField] private AudioClip[] gunShotSounds;      // 4 clips para la pistola
    // [SerializeField] private AudioClip[] shotgunShotSounds;  // 4 clips para la escopeta
    // [SerializeField, Range(0f, 1f)] private float shotVolume = 1f;//el volumen no lo toqueis porque ya se gestiona en los settings

    // private AudioSource audioSource;

    Transform ICardAction.PlayerTransform { get => playerTransform; set => playerTransform = value; }

    // private void Awake()
    // {
    //     audioSource = GetComponent<AudioSource>();
    //     if (audioSource == null)
    //     {
    //         audioSource = gameObject.AddComponent<AudioSource>();
    //         audioSource.playOnAwake = false;
    //     }
    // }

    public void ExecuteCardAction(CardObject cardObj)
    {
        switch (typeOfGun)
        {
            case TypeOfGun.Gun:
                ShootGun();
                break;
            case TypeOfGun.Shotgun:
                ShootShotgun();
                break;
        }

        cardObj.UsingCard = false;
    }

    private void ShootGun()
    {
        // // Sonido aleatorio de pistola
        // PlayRandomSound(gunShotSounds);
        GetComponent<ASoundPlayer>().PlayRandomSound();

        GameObject bPrefab = Instantiate(bulletPrefab, playerTransform.position + playerTransform.forward * 2, playerTransform.rotation);
        bPrefab.SetActive(true);
        ParticleSystem ps = bPrefab.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        var cam = playerTransform.parent.GetComponent<CameraController>();
        if (cam != null) cam.Shake(0.4f, 2, 2);

        Destroy(ps.gameObject, 5);
    }

    private void ShootShotgun()
    {
        // Sonido aleatorio de escopeta (como la pistola)
        // PlayRandomSound(shotgunShotSounds);
        GetComponent<ASoundPlayer>().PlayRandomSound();

        for (int i = 0; i < bulletsCount; i++)
        {
            float angle = -spreadAngle / 2 + (spreadAngle / (bulletsCount - 1)) * i;
            Quaternion rotation = Quaternion.Euler(playerTransform.eulerAngles + new Vector3(0, angle, 0));
            GameObject bPrefab = Instantiate(bulletPrefab, playerTransform.position + playerTransform.forward * 2, rotation);
            bPrefab.SetActive(true);
            ParticleSystem ps = bPrefab.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();

            var cam = playerTransform.parent.GetComponent<CameraController>();
            if (cam != null) cam.Shake(0.5f, 4, 4);

            Destroy(ps.gameObject, 5);
        }
    }

    // private void PlayRandomSound(AudioClip[] clips)
    // {
    //     if (clips != null && clips.Length > 0 && audioSource != null)
    //     {
    //         AudioClip clip = clips[Random.Range(0, clips.Length)];
    //         audioSource.PlayOneShot(clip, shotVolume);
    //     }
    // }
}
