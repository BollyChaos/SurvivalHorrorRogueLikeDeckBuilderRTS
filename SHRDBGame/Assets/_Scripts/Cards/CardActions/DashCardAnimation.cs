using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCardAnimation : MonoBehaviour , ICardAction
{
 [Header("Dash Settings")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private AnimationCurve dashSpeedCurve;
    [SerializeField] private GameObject dashParticles;

    private Rigidbody rb;
    private bool canDash = true;
    private Vector3 dashDirection;

    public Transform PlayerTransform { get => playerTransform; set => playerTransform=value; }
    private Transform playerTransform;
    void Start()
    {
        dashParticles.SetActive(false);
    }



    public void ExecuteCardAction(CardObject cardObj)
    {
         rb = playerTransform.GetComponent<Rigidbody>();
        if (dashSpeedCurve == null || dashSpeedCurve.length == 0)
        {
            // Curva lineal por defecto
            dashSpeedCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        }
                    StartCoroutine(DoDash());
        cardObj.UsingCard = false;
    }


    private IEnumerator DoDash()
    {
        canDash = false;
        GameObject dp = Instantiate(dashParticles, playerTransform.position, playerTransform.rotation);
        dp.transform.SetParent(playerTransform);
        dp.SetActive(true);
        dashDirection = transform.forward.normalized; // o la dirección del movimiento actual
        //float timer = 0f;

        rb.velocity = Vector3.zero;
        rb.useGravity = false;

        //refinar si da tiempo
        // while (timer < dashDuration)
        // {
        //     Debug.Log("Dasheando");
        //     float t = timer / dashDuration;
        //     float currentSpeed = dashForce * dashSpeedCurve.Evaluate(t);
        //     rb.velocity = dashDirection * currentSpeed;

        //     timer += Time.deltaTime;
        //     yield return null;
        // }
        rb.AddForce(playerTransform.forward*dashForce, ForceMode.Impulse);

        rb.velocity = Vector3.zero;
        rb.useGravity = true;

        yield return new WaitForSeconds(dashCooldown);
        Destroy(dp.gameObject);
        canDash = true;
    }

}
