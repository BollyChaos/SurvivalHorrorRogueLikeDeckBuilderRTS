using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Stats stats;
    private bool godStatus = false;
    private bool unlimitedDamage = false;
    private void Start()
    {
        stats.CurrentHealth = stats.MaxHealth;
        SettingsManager.Instance.onSettingsChange.AddListener(onSettingsChange);
        onSettingsChange();
    }
    private void onSettingsChange()
    {
        godStatus = SettingsManager.Instance.GetValue<bool>("Invincible");
        unlimitedDamage = SettingsManager.Instance.GetValue<bool>("UnlimitedDamage");
        stats.MaxHealth = SettingsManager.Instance.GetValue<float>("PlayerHealth");

    }
    public void Attack(EnemyCombat enemy)
    {
        if (enemy != null && enemy.stats.IsAlive())
        {
            //tener en cuenta el daño ilimitado para cuando se haga esto hacer 999999 de daño
            //enemy.stats.TakeDamage(stats.Attack);
            Debug.Log($"Player attacked enemy. Enemy health: {enemy.stats.CurrentHealth}");
        }
    }
    [Range(0, 1)]
    [SerializeField] float ammounthealth = 1;
     [ContextMenu("PruebaSalud")]
    public void TrySetHealth()
    {
        stats.CurrentHealth = stats.MaxHealth * ammounthealth;
        SetHealth(ammounthealth);
    }
    private void SetHealth(float healthAmmount)//valor normalizado por favor :)
    {
        if (godStatus) return;//un dios ni siente ni padece
        UIManager.Instance.SetPlayerHealthUI(healthAmmount);
    }
    public void TakeDamage(float amount)
    {
        stats.TakeDamage(amount);
        transform.parent.GetComponent<CameraController>().Shake(1.5f,(amount/stats.MaxHealth)*6f,(amount/stats.MaxHealth)*6f);
        SetHealth(stats.CurrentHealth / stats.MaxHealth);

    }

    //Metodo para curar
    public void Heal(float amount)
    {
        stats.Heal(amount);
        SetHealth(stats.CurrentHealth / stats.MaxHealth);
    }
}
