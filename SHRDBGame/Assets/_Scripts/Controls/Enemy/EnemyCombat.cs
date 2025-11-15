using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    public Stats stats;

    public float damageCooldown = 1f; // Tiempo entre ataques
    private float lastAttackTime = 0f;

    private void OnColissionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyVision"))
        {return;}
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
            //Debug.Log($"Enemy hit player. Player health: {player.stats.CurrentHealth}");
        }
    }
    public void takeDamage(float amount)
    {
        stats.TakeDamage(amount);
        //Debug.Log($"Enemy took damage. Current health: {stats.CurrentHealth}");
        if (!stats.IsAlive())
        {
            //Desactivar Enemigo
            gameObject.SetActive(false);
            //Debug.Log("Enemy died.");
        }
    }
}