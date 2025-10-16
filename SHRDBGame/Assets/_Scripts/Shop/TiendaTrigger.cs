using UnityEngine;

public class TiendaTrigger : MonoBehaviour
{
    [Header("Referencias UI")]
    public GameObject textoInteraccion;
    public GameObject panelTienda;

    private bool jugadorEnRango = false;

    void Start()
    {
        if (textoInteraccion != null)
            textoInteraccion.SetActive(false);

        if (panelTienda != null)
            panelTienda.SetActive(false);
    }

    void Update()
    {
        if (jugadorEnRango && Input.GetKeyDown(KeyCode.E))
        {
            AbrirTienda();
        }

        if (panelTienda != null && panelTienda.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarTienda();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (root.CompareTag("Player"))
        {
            jugadorEnRango = true;

            if (textoInteraccion != null)
                textoInteraccion.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (root.CompareTag("Player"))
        {
            jugadorEnRango = false;

            if (textoInteraccion != null)
                textoInteraccion.SetActive(false);

            if (panelTienda != null)
                panelTienda.SetActive(false);
        }
    }

    void AbrirTienda()
    {
        if (panelTienda != null)
            panelTienda.SetActive(true);

        if (textoInteraccion != null)
            textoInteraccion.SetActive(false);
    }

    public void CerrarTienda()
    {
        if (panelTienda != null)
            panelTienda.SetActive(false);

        if (jugadorEnRango && textoInteraccion != null)
            textoInteraccion.SetActive(true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        SphereCollider sc = GetComponent<SphereCollider>();
        if (sc != null)
            Gizmos.DrawWireSphere(transform.position, sc.radius);
    }
}
