using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFireCardAction : MonoBehaviour, ICardAction
{
    [SerializeField] GameObject wildFirePrefab;
    [SerializeField] int nWildFires = 5;
    [SerializeField] float radius;
    [SerializeField] float speed = 2f;

    [Header("Audio")]
    [SerializeField] private ASoundPlayer soundPlayer;

    private Transform playerTransform;
    public Transform PlayerTransform { get => playerTransform; set => playerTransform = value; }

    private List<GameObject> activeFires = new List<GameObject>();

    void Start()
    {
        wildFirePrefab.SetActive(false);
    }

    public void ExecuteCardAction(CardObject cardObj)
    {
        SpawnAround();
        cardObj.UsingCard = false;

        // reproducir audio en bucle mientras exista al menos un WildFire
        if (soundPlayer != null && activeFires.Count > 0)
        {
            soundPlayer.PlayLoop();
        }
    }

    public void SpawnAround()
    {
        Vector3 center = playerTransform.position;
        for (int i = 0; i < nWildFires; i++)
        {
            float angle = i * Mathf.PI * 2f / nWildFires;
            Vector3 pos = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y,
                center.z + Mathf.Sin(angle) * radius
            );

            GameObject wildFire = Instantiate(wildFirePrefab, pos + Vector3.up, Quaternion.identity);
            wildFire.transform.SetParent(playerTransform);
            wildFire.SetActive(true);

            wildFire.GetComponent<Orbit>().InitOrbit(playerTransform, radius, speed);

            // añadir a la lista de activos
            activeFires.Add(wildFire);

            // cuando se destruya, lo eliminamos de la lista
            WildFireInstance wf = wildFire.AddComponent<WildFireInstance>();
            wf.Init(this, wildFire);
        }
    }

    // llamado por cada WildFire cuando se destruye
    public void NotifyFireDestroyed(GameObject fire)
    {
        if (activeFires.Contains(fire))
        {
            activeFires.Remove(fire);
        }

        // si ya no queda ninguna, detener el loop
        if (activeFires.Count == 0 && soundPlayer != null)
        {
            soundPlayer.StopLoop();
        }
    }
}

// helper para notificar destrucción
public class WildFireInstance : MonoBehaviour
{
    private WildFireCardAction parent;
    private GameObject self;

    public void Init(WildFireCardAction parentAction, GameObject selfObj)
    {
        parent = parentAction;
        self = selfObj;
    }

    private void OnDestroy()
    {
        if (parent != null)
            parent.NotifyFireDestroyed(self);
    }
}