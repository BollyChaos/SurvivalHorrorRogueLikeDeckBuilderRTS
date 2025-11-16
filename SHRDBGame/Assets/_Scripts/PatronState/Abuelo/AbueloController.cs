using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AbueloController : EnemyController
{
    //Atributos
     private List<Transform> waypoints = new List<Transform>();
    private bool _salonAbierto = false;
    [SerializeField] private float hearingRange = 50f;
    private Vector3? lastHeardSoundPosition = null;
    private int currentWaypointIndex = 0;
    private float _restDuration = 1.5f;
    private float damage = 40f;
    [SerializeField] GameObject slashPrefab;
    //Metodos
    private void Awake()
    {

        base.Awake();
        
        // Buscar el GameObject con nombre "WaypointsAbuelo" y obtener sus hijos como waypoints
        GameObject waypointsContainer = GameObject.Find("WaypointsAbuelo");
        if (waypointsContainer != null)
        {
            waypoints.Clear();
            for (int i = 0; i < waypointsContainer.transform.childCount; i++)
            {
                waypoints.Add(waypointsContainer.transform.GetChild(i));
            }
            //Debug.Log($"Abuelo: {waypoints.Count} waypoints cargados desde {waypointsContainer.name}");
        }
        else
        {
            //Debug.LogError("No se encontró el GameObject 'WaypointsAbuelo' en la escena");
        }

        SetState(new AbueloPatrolling(this));
    }
    private void OnEnable()
    {
        SetState(new AbueloPatrolling(this));
    }
    #region Waypoints
    public override Transform GetCurrentWaypoint() {
        return waypoints[currentWaypointIndex];
    }
    public override void NextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
    }
    public override int GetCurrentWaypointIndex()
    {
        return currentWaypointIndex;
    }
    #endregion

    #region sonidos

    public void OnSoundHeard(Vector3 soundPosition)
    {
        float distance = Vector3.Distance(transform.position, soundPosition);
        if (distance <= hearingRange)
        {
            lastHeardSoundPosition = soundPosition;
            SetState(new AbueloChasing(this, soundPosition));
        }
    }
    public Vector3? GetLastHeardSoundPosition()
    {
        return lastHeardSoundPosition;
    }

    #endregion

    #region salon
    public override void SetSalonAbierto(bool estado)
    {
        _salonAbierto = estado;
    }
    public override bool IsSalonAbierto()
    {
        return _salonAbierto;
    }
    #endregion

    #region rest
    public override float GetRestDuration()
    {
        return _restDuration;
    }
    #endregion

    #region ataque
    public override void AttackPlayer()
    {
        ///Se Crea el slash para que el enemigo ataque
        GameObject sPrefab = Instantiate(slashPrefab, transform.position + transform.forward * 2, transform.rotation);
        sPrefab.SetActive(true);
        ParticleSystem ps = sPrefab.GetComponent<ParticleSystem>();
        ps.Play();

        PrefabDamage slash = sPrefab.GetComponent<PrefabDamage>();
        if (slash != null)
        {
            slash.Initialize(damage, "Player");
        }
        Destroy(sPrefab, 5f);
    }
    #endregion
    
}
