using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildFireCardAction : ACardAction
{
    [SerializeField] GameObject wildFirePrefab;
    [SerializeField] int nWildFires = 5;
    [SerializeField] float radius;
    [SerializeField] float speed = 2f;

    [Header("Audio")]
    [SerializeField] private ASoundPlayer soundPlayer;
    private List<GameObject> activeFires = new List<GameObject>();

    void Start()
    {
        wildFirePrefab.SetActive(false);
    }

    public override void ExecuteCardAction(CardObject cardObj)
    {
        SpawnAround();
        cardObj.UsingCard = false;

        if (soundPlayer != null && activeFires.Count > 0)
        {
            soundPlayer.PlayLoop();
        }
    }

    public void SpawnAround()
    {
        Vector3 center = PlayerTransform.position;

        for (int i = 0; i < nWildFires; i++)
        {
            float angle = i * Mathf.PI * 2f / nWildFires;
            Vector3 pos = new Vector3(
                center.x + Mathf.Cos(angle) * radius,
                center.y,
                center.z + Mathf.Sin(angle) * radius
            );

            GameObject wildFire = ObjectPoolManager.Instance.Get("WildFirePrefab",PlayerTransform);

            // Instantiate(wildFirePrefab, pos + Vector3.up, Quaternion.identity);

            // wildFire.transform.SetParent(PlayerTransform);
            wildFire.transform.position=pos+Vector3.up;
            wildFire.transform.rotation=Quaternion.identity;
            wildFire.SetActive(true);

            wildFire.GetComponent<Orbit>().InitOrbit(PlayerTransform, radius, speed);

            activeFires.Add(wildFire);

            // WildFireInstance wf = wildFire.AddComponent<WildFireInstance>();
            // wf.Init(this, wildFire);

           // Destroy(wildFire, 20f);
           DelayedActions.Do(wildFire.GetComponent<IPoolableObject>().Release,duration,this);
        }
    }

    public void NotifyFireDestroyed(GameObject fire)
    {
        if (activeFires.Contains(fire))
        {
            activeFires.Remove(fire);
        }

        if (activeFires.Count == 0 && soundPlayer != null)
        {
            soundPlayer.StopLoop();
        }
    }

    public override void ResetCardAction()
    {
       
    }
}

// public class WildFireInstance : MonoBehaviour Edu no programes mas
// {
//     private WildFireCardAction parent;
//     private GameObject self;

//     public void Init(WildFireCardAction parentAction, GameObject selfObj)
//     {
//         parent = parentAction;
//         self = selfObj;
//     }

//     private void OnDestroy()
//     {
//         if (parent != null)
//             parent.NotifyFireDestroyed(self);
//     }
// }