using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCardAction : MonoBehaviour, ICardAction
{
    private enum TypeOfGun { Gun, Shotgun }
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] public Transform playerTransform;
    [SerializeField] private TypeOfGun typeOfGun;
    Transform ICardAction.PlayerTransform { get => playerTransform; set => playerTransform = value; }
    [Header("Gun Settings")]

    [SerializeField] float spreadAngle = 60f;
    [SerializeField] int bulletsCount = 4;

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
    void ShootShotgun()
    {
        //Disparar varios proyectiles en abanico
        for (int i = 0; i < bulletsCount; i++)
        {
            float angle = -spreadAngle / 2 + (spreadAngle / (bulletsCount - 1)) * i;
            Quaternion rotation = Quaternion.Euler(playerTransform.eulerAngles + new Vector3(0, angle, 0));
            GameObject bPrefab = Instantiate(bulletPrefab, playerTransform.position + playerTransform.forward * 2, rotation);
            bPrefab.SetActive(true);
            ParticleSystem ps = bPrefab.GetComponent<ParticleSystem>();
            ps.Play();
            // Inicia la corrutina que destruye el sistema cuando acabe

            Destroy(ps.gameObject, 5); // Destruye el objeto del proyectil después de 5 segundos como medida de seguridad
        }

    }
    void ShootGun()
    {
        GameObject bPrefab = Instantiate(bulletPrefab, playerTransform.position + playerTransform.forward * 2, playerTransform.rotation);
        bPrefab.SetActive(true);
        ParticleSystem ps = bPrefab.GetComponent<ParticleSystem>();
        ps.Play();
        // Inicia la corrutina que destruye el sistema cuando acabe

        Destroy(ps.gameObject, 5); // Destruye el objeto del proyectil después de 5 segundos como medida de seguridad
    }

}






