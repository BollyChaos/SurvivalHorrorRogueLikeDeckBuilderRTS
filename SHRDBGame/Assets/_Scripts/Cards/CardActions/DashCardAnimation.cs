using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class DashCardAnimation : ACardAction
{
    //TODO: para de destruir cosas macho
    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    //[SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private AnimationCurve dashSpeedCurve;
    [SerializeField] private GameObject dashParticles;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1f;

    private Rigidbody rb;
    private Vector3 dashDirection;

    private ASoundPlayer soundPlayer;
    private PlayerCombat playerCombat;


    void Start()
    {
        soundPlayer = GetComponent<ASoundPlayer>();

        if (dashParticles != null)
            dashParticles.SetActive(false);
    }

    public override void ExecuteCardAction(CardObject cardObj)
    {
        rb = PlayerTransform.GetComponent<Rigidbody>();
        playerCombat = PlayerTransform.GetComponent<PlayerCombat>();

        if (dashSpeedCurve == null || dashSpeedCurve.length == 0)
        {
            dashSpeedCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        }

        StartCoroutine(DoDash());

        if (playerCombat != null)
            playerCombat.ActivateTemporaryInvincibility(invincibilityDuration);

        cardObj.UsingCard = false;
    }

    private IEnumerator DoDash()
    {

        GameObject dp = null;
        if (dashParticles != null)
        {
            dp = Instantiate(dashParticles, PlayerTransform.position, PlayerTransform.rotation);
            dp.transform.SetParent(PlayerTransform);
            dp.SetActive(true);
        }

        if (soundPlayer != null)
            soundPlayer.PlayRandomSound();

        dashDirection = transform.forward.normalized;

        rb.velocity = Vector3.zero;
        rb.useGravity = false;

        rb.AddForce(PlayerTransform.forward * dashForce, ForceMode.Impulse);

        rb.velocity = Vector3.zero;
        rb.useGravity = true;

        yield return new WaitForSeconds(dashCooldown);

        if (dp != null) Destroy(dp.gameObject);
    }

    public override void ResetCardAction()
    {
    }


}