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

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.GetComponent<Interactor>() != null)
        {
            if (other.transform.GetComponent<Interactor>().isInteracting)
            {
                UIManager.Instance.AbrirPanel();
            }
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        SphereCollider sc = GetComponent<SphereCollider>();
        if (sc != null)
            Gizmos.DrawWireSphere(transform.position, sc.radius);
    }
}
