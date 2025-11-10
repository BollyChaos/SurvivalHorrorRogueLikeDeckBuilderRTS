using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabDamage : MonoBehaviour
{
    private float damage = 0f;
    private string targetTag = "Enemy";

    private void Start()
    {
      //lo voy a destruir como apaño en slashcardaction
       // Destroy(gameObject, lifetime); por que???? ponlo en una funcion y controlas cuando se destruye no solo nada mas crearlo
    }


    public void Initialize(float dmg, string targetTag = "Enemy")
    {
        damage = dmg;
        this.targetTag = targetTag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            EnemyCombat enemy = other.GetComponent<EnemyCombat>();
            if (enemy != null)
            {
                enemy.stats.TakeDamage(damage);
                Debug.Log($"{other.name} took {damage} damage from attack!");
            }
        }
    }
}
