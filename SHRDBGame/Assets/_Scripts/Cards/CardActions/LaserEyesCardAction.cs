using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
[RequireComponent(typeof(AudioSource))]
public class LaserEyesCardAction : ACardAction
{
    [SerializeField] private GameObject laserEyesPrefab;
    [SerializeField, ReadOnly] GameObject laserEyesLeft;
    [SerializeField, ReadOnly] GameObject laserEyesRight;
    [SerializeField] float damage = 65f;
    [Header("Audio")]
    [SerializeField] private ASoundPlayer laserSound;

    private void Awake()
    {
        if (laserSound == null)
            laserSound = GetComponent<ASoundPlayer>();
    }

    public override void ExecuteCardAction(CardObject cardObj)
    {
        laserSound?.PlaySound(0);

        CreateLaserEyes();
        cardObj.UsingCard = false;
    }

    void CreateLaserEyes()
    {
        if (laserEyesLeft == null)
            laserEyesLeft = Instantiate(
               laserEyesPrefab,
               PlayerTransform.position + PlayerTransform.forward * 1 + PlayerTransform.right * 0.3f,
               PlayerTransform.rotation
           );
        else
        {
            laserEyesLeft.transform.position = PlayerTransform.position + PlayerTransform.forward * 1 + PlayerTransform.right * 0.3f;
            laserEyesLeft.transform.rotation = PlayerTransform.rotation;
        }
        float totalDamage = PlayerTransform.GetComponent<PlayerCombat>().stats.AttackMultiplier * damage;
        laserEyesLeft.GetComponent<PrefabDamage>().Initialize(totalDamage, "Enemy",true);
        laserEyesLeft.transform.parent = PlayerTransform;
        laserEyesLeft.SetActive(true);

        ParticleSystem ps = laserEyesLeft.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        if (laserEyesRight == null)
            laserEyesRight = Instantiate(
               laserEyesPrefab,
               PlayerTransform.position + PlayerTransform.forward * 1 - PlayerTransform.right * 0.3f,
               PlayerTransform.rotation
           );
        else
        {
            laserEyesRight.transform.position = PlayerTransform.position + PlayerTransform.forward * 1 - PlayerTransform.right * 0.3f;
            laserEyesRight.transform.rotation = PlayerTransform.rotation;
        }
        laserEyesRight.GetComponent<PrefabDamage>().Initialize(totalDamage, "Enemy",true);

        laserEyesRight.transform.parent = PlayerTransform;
        laserEyesRight.SetActive(true);

        ParticleSystem psr = laserEyesRight.GetComponent<ParticleSystem>();
        if (psr != null) psr.Play();

        var cam = PlayerTransform.parent.GetComponent<CameraController>();
        if (cam != null)
            cam.Shake(0.4f, 2, 5);

        DelayedActions.Do(() => laserEyesLeft.SetActive(false), duration, this);
        DelayedActions.Do(() => laserEyesRight.SetActive(false), duration, this);
        // Destroy(ps.gameObject, 5);
        // Destroy(psr.gameObject, 5);
    }

    public override void ResetCardAction()
    {

    }


}