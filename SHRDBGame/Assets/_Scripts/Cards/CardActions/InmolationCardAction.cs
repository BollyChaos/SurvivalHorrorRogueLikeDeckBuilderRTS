using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class InmolationCardAction : ACardAction
{

    private ASoundPlayer soundPlayer;

    void Start()
    {
        soundPlayer = GetComponent<ASoundPlayer>();
    }

    public override void ExecuteCardAction(CardObject cardObj)
    {

        GameObject ep = ObjectPoolManager.Instance.Get("InmolationCardPrefab");

        ep.SetActive(true);
        ep.transform.position = PlayerTransform.position;


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
        DelayedActions.Do(() => ep.SetActive(false), duration, this);
    }

    IEnumerator PlayCameraShake()
    {
        PlayerTransform.parent.GetComponent<CameraController>().Shake(1, 8, 8);

        yield return new WaitForSeconds(1f);
        PlayerTransform.parent.GetComponent<CameraController>().Shake(.5f, 18, 18);
    }

    public override void ResetCardAction()
    {

    }
}
