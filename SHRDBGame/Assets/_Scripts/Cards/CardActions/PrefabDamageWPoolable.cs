using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PoolableObject))]
public class PrefabDamageWPoolable : PrefabDamage
{
    [SerializeField] bool GoBackToPoolAfterCollision = true;
    protected override void OnParticleCollision(GameObject other)
    {
        if (particleCollision)


            if (other.CompareTag(targetTag))
            {
                //lo que habia antes era una porqueria,Victor no vuelvas a programar xd, cambiar a clase generica combat
                // Debug.Log("He encontrado un " + targetTag);
                particleCollision = false;
                AttackOther(other);
                if (GoBackToPoolAfterCollision)
                    GetComponent<PoolableObject>().Release();

            }
    }
    protected override void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.layer == LayerMask.NameToLayer("EnemyVision")) { return; }
        if ((layerMaskIgnore.value & (1 << other.gameObject.layer)) != 0)//esto hace lo mismo y mas y ya no harcodea cosas
            return;
        if (other.CompareTag(targetTag))
        {
            //Debug.Log("He encontrado un " + targetTag);
            AttackOther(other.gameObject);
            if (GoBackToPoolAfterCollision)
                GetComponent<PoolableObject>().Release();

        }
    }
}
