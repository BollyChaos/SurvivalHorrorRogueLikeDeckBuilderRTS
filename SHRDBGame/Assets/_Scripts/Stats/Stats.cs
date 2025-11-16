using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    [Header("Atributos Basicos")]
    [SerializeField]
    private float maxHealth = 100f;
    [SerializeField]
    public float MaxHealth { get => maxHealth; set => maxHealth=value; }//esto lo necesito, pero solo se lee
    [SerializeField]
    private float currentHealth = 100f;//a ver, hazlas privadas o quitas el proposito de tener funciones para quitar o tomar vida
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    [SerializeField]
    private bool invencibility = false;
    public bool Invencibility{ get => invencibility; set => invencibility = value; }
    public bool IsInvincible { get => invencibility; }
    [SerializeField]
    private float attack = 10f;
    [SerializeField]
    public float baseAttack{ get => attack; set => attack = value; }
    [SerializeField]
    public float Attack{ get { return attack* attackMultiplier; } }
    [SerializeField]
    public float AttackMultiplier { get => attackMultiplier; set => attackMultiplier = value; }
    [SerializeField]
    private float attackMultiplier = 1f;
    [SerializeField]
    private float speedMultiplier = 1f;
    public float SpeedMultiplier{ get => speedMultiplier; }
    //Metodo para aplicar da�o teniendo en cuenta la defensa
    public void TakeDamage(float amount)
    {
        if (IsInvincible) return;
        currentHealth = currentHealth - amount;
    }

    //Metodo para curar
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }
    public void ChangeSpeedMultiplier(float sM)
    {
        speedMultiplier = sM;
    }
   public void ResetStats()
    {
        currentHealth = maxHealth;
        attackMultiplier = 1f;
        speedMultiplier = 1f;
        invencibility = false;
    }
}