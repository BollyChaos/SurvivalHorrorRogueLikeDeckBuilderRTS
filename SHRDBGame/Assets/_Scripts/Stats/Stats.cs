using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stats
{
    [Header("Atributos Basicos")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;
    public float attack = 10f;

    //Metodo para aplicar daño teniendo en cuenta la defensa
    public void TakeDamage(float amount)
    {
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
}