using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEyesCardAction : MonoBehaviour,ICardAction
{
    private Transform playerTransform;
    public Transform PlayerTransform { get => playerTransform; set => playerTransform=value; }

    [SerializeField]
    private GameObject laserEyesPrefab;    

    public void ExecuteCardAction(CardObject cardObj)
    {
       CreateLaserEyes();
       cardObj.UsingCard=false;
    }
void CreateLaserEyes()
    {
        GameObject laserEyesLeft = Instantiate(laserEyesPrefab, playerTransform.position + playerTransform.forward * 1+playerTransform.right*0.5f, playerTransform.rotation);
        laserEyesLeft.transform.parent = playerTransform;
        laserEyesLeft.SetActive(true);

        ParticleSystem ps = laserEyesLeft.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();

        GameObject laserEyesRight = Instantiate(laserEyesPrefab, playerTransform.position + playerTransform.forward * 1-playerTransform.right*0.5f, playerTransform.rotation);
        laserEyesRight.transform.parent = playerTransform;
        laserEyesRight.SetActive(true);

         ParticleSystem psr = laserEyesRight.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();
        var cam = playerTransform.parent.GetComponent<CameraController>();
        if (cam != null) cam.Shake(0.4f, 2, 5);

        Destroy(ps.gameObject, 5);
        Destroy(psr.gameObject, 5);

    }
    
   
}
