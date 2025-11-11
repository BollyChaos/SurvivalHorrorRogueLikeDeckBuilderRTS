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
            //enemy.stats.TakeDamage(stats.Attack);
            Debug.Log($"Player attacked enemy. Enemy health: {enemy.stats.CurrentHealth}");
        }
    }
    [Range(0, 1)]
    [SerializeField] float ammounthealth = 1;
     [ContextMenu("PruebaSalud")]
    public void TrySetHealth()
    {
        SetHealth(ammounthealth);
    }
    public void SetHealth(float healthAmmount)//valor normalizado por favor :)
    {
        UIManager.Instance.SetPlayerHealthUI(healthAmmount);
    }
    public void TakeDamage(float amount)
    {
        stats.TakeDamage(amount);
        SetHealth(stats.CurrentHealth / stats.MaxHealth);

    }

    //Metodo para curar
    public void Heal(float amount)
    {
        stats.Heal(amount);
        SetHealth(stats.CurrentHealth / stats.MaxHealth);
    }
}
