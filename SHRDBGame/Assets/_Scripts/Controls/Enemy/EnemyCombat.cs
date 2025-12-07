using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public Stats stats;

    public float damageCooldown = 1f;
    private float lastAttackTime = 0f;

    private TioController tioController;

    protected void OnEnable()
    {
        stats.ResetStats();
        tioController = GetComponent<TioController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyVision")) return;

        PlayerCombat player = collision.gameObject.GetComponent<PlayerCombat>();

        if (player != null && Time.time >= lastAttackTime + damageCooldown)
        {
            Attack(player);
            lastAttackTime = Time.time;
        }
    }

    public void Attack(PlayerCombat player)
    {
        if (player != null && player.stats.IsAlive())
        {
            player.TakeDamage(stats.Attack);

            if (tioController != null)
            {
                tioController.AttackPlayer();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        stats.TakeDamage(amount);

        if (!stats.IsAlive())
        {
            if (tioController != null)

            gameObject.GetComponent<EnemyController>().ShootDrops();
            gameObject.SetActive(false);
        }
    }
}