using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        boxCollider.enabled = false;
        StartCoroutine(MeteorRoutine());
    }

    private IEnumerator MeteorRoutine()
    {
        // Esperar al impacto (sub-emitter o animación)
        yield return new WaitForSeconds(0.5f);

        // Daño
        boxCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        boxCollider.enabled = false;
    }
}
