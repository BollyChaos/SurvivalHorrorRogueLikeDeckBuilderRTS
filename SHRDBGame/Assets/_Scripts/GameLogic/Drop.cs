using UnityEngine;

public class Drop : MonoBehaviour
{
    private enum DropType { Health, Money }
    [SerializeField]
    private DropType dropType;
    private bool canGive = true;
    private enum DropSize { Small=0, Medium=1, Large=2 }
    [SerializeField]
    private DropSize dropSize = DropSize.Small;
    [Header("Magnet variables")]
    [SerializeField] private float attractionRange = 5f;
    [SerializeField] private float attractionForce = 10f;
    [SerializeField] private float pickupRadius = 0.5f;
    [SerializeField] private string playerTag = "Player";

    private Transform player;

    void Start()
    {

        InitDrop();
    }
    public void InitDrop()
    {
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
        float size = 1 + (int)dropSize*.5f;
        transform.localScale = new Vector3(size, size, size);
    }
    void Update()
    {
        if (!canGive) return;
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Si el jugador está dentro del rango de atracción
        if (distance < attractionRange)
        {
            // Movimiento suave hacia el jugador
            Vector3 direction = (player.position - transform.position).normalized;
            float step = attractionForce * Time.deltaTime * (1 - distance / attractionRange);
            transform.position += direction * step;
        }

        // Si está suficientemente cerca, recoger
        if (distance < pickupRadius)
        {
            OnPickup();
        }
    }

    void OnPickup()
    {
        // Aquí podrías añadir sonido, partículas o sumar puntos
        Debug.Log($"{name} recogido por el jugador.");
        switch (dropType)
        {
            case DropType.Health:
                float percentage = (.2f + (int)dropSize * 0.1f);//se cura un 20% por defecto, 30 y 40
                float healAmount = player.transform.GetComponentInChildren<PlayerCombat>().stats.MaxHealth * percentage;
                player.transform.GetComponentInChildren<PlayerCombat>().Heal(healAmount);
                break;
            case DropType.Money:
                player.transform.GetComponentInChildren<Economy>().AddCoins((int)dropSize + 1);
                break;

        }
        GetComponent<AudioSource>()?.Play();//si tenemos sonido usarlo
        canGive = false;
        Destroy(gameObject,0.1f);

    }
}
