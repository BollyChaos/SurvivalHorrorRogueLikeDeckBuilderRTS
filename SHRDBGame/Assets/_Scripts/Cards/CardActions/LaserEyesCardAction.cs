using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
[RequireComponent(typeof(AudioSource))]
public class LaserEyesCardAction : ACardAction
{
    [SerializeField] private GameObject laserEyesPrefab;

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
        GameObject laserEyesLeft = Instantiate(
            laserEyesPrefab,
            PlayerTransform.position + PlayerTransform.forward * 1 + PlayerTransform.right * 0.3f,
            PlayerTransform.rotation
        );

        laserEyesLeft.transform.parent = PlayerTransform;
        laserEyesLeft.SetActive(true);

        ParticleSystem ps = laserEyesLeft.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        GameObject laserEyesRight = Instantiate(
            laserEyesPrefab,
            PlayerTransform.position + PlayerTransform.forward * 1 - PlayerTransform.right * 0.3f,
            PlayerTransform.rotation
        );

        laserEyesRight.transform.parent = PlayerTransform;
        laserEyesRight.SetActive(true);

        ParticleSystem psr = laserEyesRight.GetComponent<ParticleSystem>();
        if (psr != null) psr.Play();

        var cam = PlayerTransform.parent.GetComponent<CameraController>();
        if (cam != null)
            cam.Shake(0.4f, 2, 5);

        Destroy(ps.gameObject, 5);
        Destroy(psr.gameObject, 5);
    }

    public override void ResetCardAction()
    {
      
    }


}