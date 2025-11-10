using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class TrapPrefab : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Animator animator;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GetComponent<PrefabDamage>().Tag))
        {
            Debug.Log("Hola");
            animator.SetBool("SetTrap", true);
             Destroy(gameObject,5f);//esto se puede cambiar despues
        }
       
    }
}
