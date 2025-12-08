using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
[RequireComponent(typeof(AudioSource))]
public class LaserEyesCardAction : MonoBehaviour, ICardAction
{
    private Transform playerTransform;
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }

    [SerializeField] private GameObject laserEyesPrefab;

    [Header("Audio")]
    [SerializeField] private ASoundPlayer laserSound;

    private void Awake()
    {
        if (laserSound == null)
            laserSound = GetComponent<ASoundPlayer>();
    }

    public void ExecuteCardAction(CardObject cardObj)
    {
        laserSound?.PlaySound(0);

        CreateLaserEyes();
        cardObj.UsingCard = false;
    }

    void CreateLaserEyes()
    {
        GameObject laserEyesLeft = Instantiate(
            laserEyesPrefab,
            playerTransform.position + playerTransform.forward * 1 + playerTransform.right * 0.3f,
            playerTransform.rotation
        );

        laserEyesLeft.transform.parent = playerTransform;
        laserEyesLeft.SetActive(true);

        ParticleSystem ps = laserEyesLeft.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        GameObject laserEyesRight = Instantiate(
            laserEyesPrefab,
            playerTransform.position + playerTransform.forward * 1 - playerTransform.right * 0.3f,
            playerTransform.rotation
        );

        laserEyesRight.transform.parent = playerTransform;
        laserEyesRight.SetActive(true);

        ParticleSystem psr = laserEyesRight.GetComponent<ParticleSystem>();
        if (psr != null) psr.Play();

        var cam = playerTransform.parent.GetComponent<CameraController>();
        if (cam != null)
            cam.Shake(0.4f, 2, 5);

        Destroy(ps.gameObject, 5);
        Destroy(psr.gameObject, 5);
    }
}