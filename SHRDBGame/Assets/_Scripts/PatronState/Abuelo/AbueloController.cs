using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AbueloController : EnemyController
{
    //Atributos
    [SerializeField] private Transform[] waypoints;
    private bool _salonAbierto = false;
    [SerializeField] private float hearingRange = 10f;
    private Vector3? lastHeardSoundPosition = null;
    private int currentWaypointIndex = 0;
    private float _restDuration = 1.5f;
    //Metodos
    private void Awake()
    {
        base.Awake();
        SetState(new AbueloPatrolling(this));
    }
    #region Waypoints
    public override Transform GetCurrentWaypoint() {
        return waypoints[currentWaypointIndex];
    }
    public override void NextWaypoint()
    {
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
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
}
