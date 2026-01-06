using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class VecinoPatrolling : AEnemyState
{
    //atributos
    private Transform _currentTransform;
    private Vector3 _destination;
    private bool _hasDestination = false;
    private float patrolSpeed;
    private NavMeshAgent _agent;
    //Metodos
    public VecinoPatrolling(IEnemy enemy) : base(enemy) { }

    public override void Enter()
    {
        _currentTransform = enemy.GetGameObject().transform;
        _destination = _currentTransform.position;

        //Debug.Log("Entering Patrol State");
        patrolSpeed = enemy.GetPatrolSpeed();
        _agent = enemy.GetNavMeshAgent();
        _agent.isStopped = false;
        _agent.speed = patrolSpeed;
        _agent.ResetPath();
        _hasDestination = false;
    }

    public override void Exit()
    {

    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
        if (_agent.velocity.magnitude < 0.1f && _agent.remainingDistance > 1f)
        {
            // Forzar un nuevo destino seguro
            _hasDestination = false;
        }
        if (enemy.PlayerAtSight() != null) //Si detecta al jugador
        {
            Debug.Log("Player Spotted, switching to Chasing Player State");
            enemy.SetState(new VecinoBattling(enemy));
        }
        else
        {
            //Comportamiento de patrulla
            if (!_hasDestination || _agent.remainingDistance < 0.5f)
            {
                Vector3 point;
                if (RandomPoint(_currentTransform.position, 15f, out point))
                {
                    _destination = point;
                    _hasDestination = true;
                    _agent.SetDestination(_destination);
                }
            }
        }
    }
    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = center;
        return false;
    }

}
