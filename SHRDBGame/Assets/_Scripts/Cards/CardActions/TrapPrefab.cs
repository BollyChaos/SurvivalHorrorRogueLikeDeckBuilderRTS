using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TrapPrefab : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnTriggerEnter(Collider other)
    {
        // Obtenemos el PrefabDamage para leer el targetTag de forma segura
        PrefabDamage damageScript = GetComponent<PrefabDamage>();
        if (damageScript != null && other.CompareTag(damageScript.TargetTag))
        {
            Debug.Log("Trap activada");

            // Activar animación
            animator.SetBool("SetTrap", true);

            // Destruir trap después de un tiempo
            Destroy(gameObject, 5f); // ajustable
        }
    }
}
