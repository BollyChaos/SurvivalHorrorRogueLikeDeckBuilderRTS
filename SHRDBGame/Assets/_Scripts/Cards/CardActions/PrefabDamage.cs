using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class PrefabDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    public float Damage=>damage;
    [SerializeField] private string targetTag;

    private ASoundPlayer soundPlayer;

    public string TargetTag => targetTag;
    public bool particleCollision=false;

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
        if (other.gameObject.layer == LayerMask.NameToLayer("EnemyVision")){return;}
        if (other.CompareTag(targetTag))
        {
            // Primero probamos si es un jugador
        PlayerCombat player = other.GetComponent<PlayerCombat>();
        if (player != null)
        {

            player.TakeDamage(damage);
            // Reproduce sonido de impacto
            if (soundPlayer != null)
            {
                soundPlayer.PlayRandomSound();
            }
            return;
        }

        // Si no es jugador, probamos si es enemigo
        EnemyCombat enemy = other.GetComponent<EnemyCombat>();
        if (enemy == null){
            enemy = other.GetComponentInParent<EnemyCombat>();
        }
        //<Debug.Log("PrefabDamage detected collision with " + other.name);
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            // Reproduce sonido de impacto
            if (soundPlayer != null)
            {
                soundPlayer.PlayRandomSound();
            }
            return;
        }

            

            //Destroy(gameObject); no lo destruyas porque no se escucha un carajo
        }
    }
     private void OnParticleCollision(GameObject other)
    {
        if(particleCollision)
//        Debug.Log("chocando con "+other.name+" con tag: "+other.tag);
        //Debug.Log(tag);

        if (other.CompareTag(targetTag))
        {

             // Primero probamos si es un jugador
        PlayerCombat player = other.GetComponent<PlayerCombat>();
        if (player != null)
        {

            player.TakeDamage(damage);
            // Reproduce sonido de impacto
            if (soundPlayer != null)
            {
                soundPlayer.PlayRandomSound();
            }
            return;
        }

        // Si no es jugador, probamos si es enemigo
        EnemyCombat enemy = other.GetComponent<EnemyCombat>();
        if (enemy == null){
            enemy = other.GetComponentInParent<EnemyCombat>();
        }
        //<Debug.Log("PrefabDamage detected collision with " + other.name);
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            // Reproduce sonido de impacto
            if (soundPlayer != null)
            {
                soundPlayer.PlayRandomSound();
            }
            return;
        }

          //  Debug.Log("dando daño soy "+name);
           other.GetComponentInParent<EnemyCombat>().TakeDamage(Damage);

        }
    }

}
