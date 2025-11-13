using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCombat : MonoBehaviour
{
    public Stats stats;
    private bool godStatus = false;
    private bool unlimitedDamage = false;

    private bool healOnDamage = false;
    public bool HealOnDamage{ get => healOnDamage; set => healOnDamage = value; }
    private bool reflectDamage = false;
    public bool ReflectDamage { get => reflectDamage; set => reflectDamage = value; }
    //curarse o recibir daño pueden ser eventos, el camera shake se puede suscribir al igual que los objetos que miren estos booleanos(para desactivarse)
    public UnityEvent<bool> OnChangeHealth;//T si ha recibido daño y F si no
    private void Start()
    {
        stats.CurrentHealth = stats.MaxHealth;
        UIManager.Instance.SetPlayerHealthUI(stats.CurrentHealth/stats.MaxHealth);
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
        if (healOnDamage)
        {
            Heal(amount);
            healOnDamage = false;
        }
        else if (reflectDamage)
        {
            reflectDamage = false;
            return;
        }
        else
        {


            stats.TakeDamage(amount);
            transform.parent.GetComponent<CameraController>().Shake(1.5f, (amount / stats.MaxHealth) * 6f, (amount / stats.MaxHealth) * 6f);
            OnChangeHealth.Invoke(true);
            if (!stats.IsAlive())
            {
                //llamar al level manager de que el jugador ha muerto
                LevelManager.Instance.EndGame();
            }

        }
        SetHealth(stats.CurrentHealth / stats.MaxHealth);

    }

    //Metodo para curar
    public void Heal(float amount)
    {
        stats.Heal(amount);
        SetHealth(stats.CurrentHealth / stats.MaxHealth);
        OnChangeHealth.Invoke(false);

    }
}
