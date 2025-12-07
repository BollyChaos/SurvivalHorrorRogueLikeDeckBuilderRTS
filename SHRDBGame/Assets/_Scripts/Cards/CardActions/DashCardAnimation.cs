using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class DashCardAnimation : MonoBehaviour, ICardAction
{
    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private AnimationCurve dashSpeedCurve;
    [SerializeField] private GameObject dashParticles;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityDuration = 1f;

    private Rigidbody rb;
    private bool canDash = true;
    private Vector3 dashDirection;

    private ASoundPlayer soundPlayer;
    private PlayerCombat playerCombat;

    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
    private Transform playerTransform;

    void Start()
    {
        soundPlayer = GetComponent<ASoundPlayer>();

        if (dashParticles != null)
            dashParticles.SetActive(false);
    }

    public void ExecuteCardAction(CardObject cardObj)
    {
        rb = playerTransform.GetComponent<Rigidbody>();
        playerCombat = playerTransform.GetComponent<PlayerCombat>();

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
        canDash = false;

        GameObject dp = null;
        if (dashParticles != null)
        {
            dp = Instantiate(dashParticles, playerTransform.position, playerTransform.rotation);
            dp.transform.SetParent(playerTransform);
            dp.SetActive(true);
        }

        if (soundPlayer != null)
            soundPlayer.PlayRandomSound();

        dashDirection = transform.forward.normalized;

        rb.velocity = Vector3.zero;
        rb.useGravity = false;

        rb.AddForce(playerTransform.forward * dashForce, ForceMode.Impulse);

        rb.velocity = Vector3.zero;
        rb.useGravity = true;

        yield return new WaitForSeconds(dashCooldown);

        if (dp != null) Destroy(dp.gameObject);
        canDash = true;
    }
}