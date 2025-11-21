using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFireCardAction : MonoBehaviour, ICardAction
{
    [SerializeField]
    GameObject wildFirePrefab;
    [SerializeField]
    int nWildFires = 5;
    [SerializeField]
    float radius;
    [SerializeField]
    float speed=2f;
    private Transform playerTransform;
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }
    public void Start()
    {
        wildFirePrefab.SetActive(false);
    }

    public void ExecuteCardAction(CardObject cardObj)
    {
        SpawnAround();
        cardObj.UsingCard = false;
    }
    public void SpawnAround()

    {
        Vector3 center = playerTransform.position;
        for (int i = 0; i < nWildFires; i++)
        {
            float angle = i * Mathf.PI * 2f / nWildFires;  // ángulo equidistante
            Vector3 pos = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y,
                center.z + Mathf.Sin(angle) * radius
            );

            GameObject wildFire=Instantiate(wildFirePrefab, pos+Vector3.up, Quaternion.identity);
            wildFire.transform.SetParent(playerTransform);
            wildFire.SetActive(true);
            wildFire.GetComponent<Orbit>().InitOrbit(playerTransform,radius,speed);

        }
    }


}
