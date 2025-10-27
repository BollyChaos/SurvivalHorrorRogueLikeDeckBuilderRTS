using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    private bool isInvincible=false;
    public float invencibilityCounter = 0.5f;
    private float counter = 0f;
    public void ChangeHealth (int amount)
    {
        currentHealth += amount;

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        if (isInvincible)
        {
            counter += Time.deltaTime;
            if (counter >= invencibilityCounter)
            {
                isInvincible = false;

                counter = 0f;
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
        {
        if (isInvincible) return;

        if (collision.gameObject.GetComponent<EnemyController>())
            {
                Debug.Log("Hola");
                ChangeHealth(-25);
                isInvincible=true;
            }
        }

    private void OnCollisionStay(Collision collision)
    {
        if (isInvincible) return;
        if (collision.gameObject.GetComponent<EnemyController>())
        {
            Debug.Log("Hola");
            ChangeHealth(-25);
            isInvincible = true;
        }
    }
}
