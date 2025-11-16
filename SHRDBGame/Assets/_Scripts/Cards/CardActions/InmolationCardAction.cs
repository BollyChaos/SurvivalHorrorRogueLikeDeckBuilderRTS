using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class InmolationCardAction : MonoBehaviour, ICardAction
{
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
    private Transform playerTransform;

    [SerializeField]
    private GameObject ExplosionPrefab;


    private ASoundPlayer soundPlayer;

    void Start()
    {
        ExplosionPrefab.SetActive(false);
        soundPlayer = GetComponent<ASoundPlayer>();
    }

    public void ExecuteCardAction(CardObject cardObj)
    {

        GameObject ep = Instantiate(ExplosionPrefab, playerTransform.position, Quaternion.identity);
        ep.SetActive(true);


        if (soundPlayer != null)
        {
            AudioSource source = soundPlayer.GetComponent<AudioSource>();
            float originalVolume = source.volume;

            source.volume = originalVolume;
            soundPlayer.PlayRandomSound();
            source.volume = originalVolume;
        }

        // Camera shake
        StartCoroutine(PlayCameraShake());

        cardObj.UsingCard = false;
        Destroy(ep, 5f);
    }

    IEnumerator PlayCameraShake()
    {
        playerTransform.parent.GetComponent<CameraController>().Shake(1, 8, 8);

        yield return new WaitForSeconds(1f);
        playerTransform.parent.GetComponent<CameraController>().Shake(.5f, 18, 18);
    }
}
