using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;

public class AbueloPatrolling : AEnemyState
{
    //atributos
    private Transform _currentTransform;
    private float _patrolSpeed;
    private Transform _currentWaypoint;
    private bool _isWaiting = false;
    private bool _salonAbierto; //cambiarlo a que lo mire del controller
    private float[] _WaitTimesF1 = { 5f, 5f, 10f };
    private float[] _WaitTimesF2 = { 5f, 5f, 5f, 10f, 15f };
    //Metodos
    public AbueloPatrolling(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _isWaiting = false;
        _currentTransform = enemy.GetGameObject().transform;
        _patrolSpeed = enemy.GetPatrolSpeed();
        _currentWaypoint = enemy.GetCurrentWaypoint();
        _salonAbierto = enemy.IsSalonAbierto();
        _currentWaypoint = enemy.GetCurrentWaypoint();
        enemy.MoveToNavMesh(_currentWaypoint.position, _patrolSpeed);
        enemy.GetNavMeshAgent().isStopped = false;
        //Debug.Log("Entering Patrolling State");
    }

    public override void Exit()
    {

    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
        if (enemy.PlayerAtSight()!=null)
        {
            enemy.SetState(new AbueloBattling(enemy));
            return;
        }
        if (_isWaiting) { return; }

        Vector3 distanceToWaypoint = _currentWaypoint.position - _currentTransform.position;
        if (distanceToWaypoint.magnitude < 0.5f)
        {
            int index = enemy.GetCurrentWaypointIndex();
            float waitTime = _salonAbierto ? _WaitTimesF2[index % _WaitTimesF2.Length] : _WaitTimesF1[index % _WaitTimesF1.Length];

            enemy.GetGameObject().GetComponent<MonoBehaviour>().StartCoroutine(WaitAtWaypoint(waitTime));
        }


    }
    private IEnumerator WaitAtWaypoint(float waitTime)
    {
        _isWaiting = true;
        enemy.GetNavMeshAgent().isStopped = true;

        yield return new WaitForSeconds(waitTime);
        enemy.GetNavMeshAgent().isStopped = false;
        enemy.NextWaypoint();
        _currentWaypoint = enemy.GetCurrentWaypoint();
        enemy.MoveToNavMesh(_currentWaypoint.position, _patrolSpeed);
        _isWaiting = false;
    }
}
