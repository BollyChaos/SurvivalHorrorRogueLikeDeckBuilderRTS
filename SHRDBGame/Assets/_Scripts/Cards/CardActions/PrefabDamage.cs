using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class PrefabDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private string targetTag;

    private ASoundPlayer soundPlayer;

    public string TargetTag => targetTag;

    private void Awake()
    {
        soundPlayer = GetComponent<ASoundPlayer>();
    }

    public void Initialize(float dmg, string tag)
    {
        damage = dmg;
        targetTag = tag;
    }

    // public void SetImpactClips(List<AudioClip> clips) no te estaban llamando porque ya se puede poner directamente en ASoundPlayer
    // {
    //     if (soundPlayer != null)
    //     {
    //         // Accede a la lista privada mediante reflexión o crea un método público en ASoundPlayer para asignar la lista
    //         soundPlayer.AssignClips(clips);
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            // Aplica daño al enemigo
            var enemy = other.GetComponent<MonoBehaviour>(); // Ajusta al script real de tu enemigo
            if (enemy != null)
            {
                var method = enemy.GetType().GetMethod("TakeDamage");
                if (method != null)
                    method.Invoke(enemy, new object[] { damage });
            }

            // Reproduce sonido de impacto
            if (soundPlayer != null)
            {
                soundPlayer.PlayRandomSound();
            }

            //Destroy(gameObject); no lo destruyas porque no se escucha un carajo
        }
    }
}
