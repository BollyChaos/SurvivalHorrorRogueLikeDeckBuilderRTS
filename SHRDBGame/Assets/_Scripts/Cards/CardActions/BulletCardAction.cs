using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCardAction : ACardAction
{
    private enum TypeOfGun { Gun, Shotgun }

   

    [SerializeField] private TypeOfGun typeOfGun;

    [Header("Gun Settings")]
    [SerializeField] float damage = 100;
    [SerializeField] float spreadAngle = 60f;
    [SerializeField] int bulletsCount = 4;


    public override void ExecuteCardAction(CardObject cardObj)
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
        // Sonido de disparo
        GetComponent<ASoundPlayer>().PlayRandomSound();

        GameObject bPrefab = ObjectPoolManager.Instance.Get("BulletCardPrefab");

        bPrefab.SetActive(true);
        bPrefab.transform.position = PlayerTransform.position + PlayerTransform.forward * 2f;
        bPrefab.transform.rotation = PlayerTransform.rotation;
        float totalDamage = damage * PlayerTransform.GetComponent<ACombat>().stats.AttackMultiplier;
        bPrefab.GetComponent<PrefabDamage>().Initialize(totalDamage, "Enemy",true);

        ParticleSystem ps = bPrefab.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        var cam = PlayerTransform.parent.GetComponent<CameraController>();
        if (cam != null) cam.Shake(0.4f, 2, 2);

        DelayedActions.Do(bPrefab.GetComponent<PoolableObject>().Release, duration, this);
        // Destroy(ps,10);
    }

    private void ShootShotgun()
    {
        // Sonido de disparo
        GetComponent<ASoundPlayer>().PlayRandomSound();

        for (int i = 0; i < bulletsCount; i++)
        {
            float angle = -spreadAngle / 2 + (spreadAngle / (bulletsCount - 1)) * i;
            Quaternion rotation = Quaternion.Euler(PlayerTransform.eulerAngles + new Vector3(0, angle, 0));
            GameObject bPrefab = ObjectPoolManager.Instance.Get("BulletCardPrefab");

            bPrefab.SetActive(true);
            bPrefab.transform.position = PlayerTransform.position + PlayerTransform.forward * 2f;
            bPrefab.transform.rotation = rotation;
            float totalDamage = damage * PlayerTransform.GetComponent<ACombat>().stats.AttackMultiplier ;

            bPrefab.GetComponent<PrefabDamage>().Initialize(totalDamage, "Enemy",true);

            ParticleSystem ps = bPrefab.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();

            var cam = PlayerTransform.parent.GetComponent<CameraController>();
            if (cam != null) cam.Shake(0.5f, 4, 4);

            DelayedActions.Do(bPrefab.GetComponent<PoolableObject>().Release, duration, this);
        }
        //DelayedActions.Do(Release,5,this);
    }



    public override void ResetCardAction()
    {
    }
}
