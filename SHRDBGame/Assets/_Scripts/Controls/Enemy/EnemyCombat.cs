using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public Stats stats;
    public float damageCooldown = 1f; //Tiempo entre cada ataque
    private float lastAttackTime = 0f;

    private void OnTriggerEnter(Collider collision)
    {
        PlayerCombat player = collision.GetComponent<PlayerCombat>();

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
            Debug.Log($"Enemy hit player. Player health: {player.stats.CurrentHealth}");
        }
    }
}
