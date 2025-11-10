using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Stats stats;

    private void Start()
    {
        stats.CurrentHealth = stats.MaxHealth;
    }

    public void Attack(EnemyCombat enemy)
    {
        if (enemy != null && enemy.stats.IsAlive())
        {
            enemy.stats.TakeDamage(stats.Attack);
            Debug.Log($"Player attacked enemy. Enemy health: {enemy.stats.CurrentHealth}");
        }
    }
}
