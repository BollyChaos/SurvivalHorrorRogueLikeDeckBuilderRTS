using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] private SphereCollider sphereCollider;
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        sphereCollider.enabled = false;
        StartCoroutine(MeteorRoutine());
    }

    private IEnumerator MeteorRoutine()
    {
        // Esperar al impacto (sub-emitter o animación)
        yield return new WaitForSeconds(1f);

        // Daño
        sphereCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        sphereCollider.enabled = false;
    }
}

