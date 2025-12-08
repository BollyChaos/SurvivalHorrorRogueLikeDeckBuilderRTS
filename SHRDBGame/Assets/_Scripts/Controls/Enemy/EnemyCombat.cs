using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public Stats stats;

    public float damageCooldown = 1f;
    private float lastAttackTime = 0f;

    private EnemyController enemyController;

    protected void OnEnable()
    {
        stats.ResetStats();
        enemyController = GetComponent<EnemyController>();
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

            if (enemyController != null)
            {
                enemyController.AttackPlayer();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        stats.TakeDamage(amount);

        if (!stats.IsAlive())
        {
            if (enemyController != null)
                gameObject.GetComponent<EnemyController>().ShootDrops();

            LevelManager.Instance.AddEnemyKill();
            gameObject.SetActive(false);
        }
    }
}