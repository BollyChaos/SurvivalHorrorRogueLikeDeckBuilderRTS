using UnityEngine;

public class TiendaTrigger : MonoBehaviour
{

    

    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (root.CompareTag("Player"))
        {
            UIManager.Instance.ShowShopText();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.GetComponent<Interactor>() != null)
        {
            if (other.transform.GetComponent<Interactor>().isInteracting)
            {
                UIManager.Instance.OpenPanel();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject root = other.transform.root.gameObject;

        if (root.CompareTag("Player"))
        {

            UIManager.Instance.HideShopText();
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
