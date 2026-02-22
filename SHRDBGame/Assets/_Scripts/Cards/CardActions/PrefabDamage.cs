
using UnityEngine;

[RequireComponent(typeof(ASoundPlayer))]
public class PrefabDamage : MonoBehaviour
{
    [SerializeField] private float damage;
    public float Damage => damage;
    [SerializeField] private string targetTag;

    [SerializeField] LayerMask layerMaskIgnore;
    private ASoundPlayer soundPlayer;

    public string TargetTag => targetTag;
    public bool particleCollision = false;

    private void Awake()
    {
        soundPlayer = GetComponent<ASoundPlayer>();
    }

    public void Initialize(float dmg, string tag)
    {
        //buscar stats de player para aplicar multiplicador de daño correspondiente
        damage = dmg;
        targetTag = tag;
    }
    public void Initialize(float dmg, string tag, bool ptCol)
    {
        particleCollision = ptCol;
        Initialize(dmg, tag);
    }
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.layer == LayerMask.NameToLayer("EnemyVision")) { return; }
        if ((layerMaskIgnore.value & (1 << other.gameObject.layer)) != 0)//esto hace lo mismo y mas y ya no harcodea cosas
            return;
        if (other.CompareTag(targetTag))
        {
            Debug.Log("He encontrado un " + targetTag);
            AttackOther(other.gameObject);

        }
    }
    private void OnParticleCollision(GameObject other)
    {
        if (particleCollision)


            if (other.CompareTag(targetTag))
            {
                //lo que habia antes era una porqueria,Victor no vuelvas a programar xd, cambiar a clase generica combat
                Debug.Log("He encontrado un " + targetTag);
                particleCollision = false;
                AttackOther(other);


            }
    }
    void AttackOther(GameObject other)
    {
        ACombat damageableObject = other.GetComponent<ACombat>();
        if (damageableObject == null)
            damageableObject = other.GetComponentInParent<ACombat>();

        if (damageableObject != null)
        {

            GameObject popUpText = ObjectPoolManager.Instance.Get("PopUpText");
            popUpText.GetComponent<DamagePopup>().Play((int)damage, other.transform.position + Vector3.up * 2);
            damageableObject.TakeDamage(damage);

            // Reproduce sonido de impacto
            if (soundPlayer != null)
            {
                soundPlayer.PlayRandomSound();
            }
            return;
        }
    }

}
