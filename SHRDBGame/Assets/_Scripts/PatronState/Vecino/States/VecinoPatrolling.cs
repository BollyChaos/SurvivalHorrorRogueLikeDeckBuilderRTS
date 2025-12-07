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
        _hasDestination = false;
        patrolSpeed = enemy.GetPatrolSpeed();
        _agent = enemy.GetNavMeshAgent();
        _agent.speed = patrolSpeed;
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
            //Debug.Log("Player Spotted, switching to Chasing Player State");
            enemy.SetState(new VecinoBattling(enemy));
        }
        else
        {
            //Comportamiento de patrulla
            if (!_hasDestination || (_destination - _currentTransform.position).magnitude < 0.5f)
            {
                Vector3 point;
                bool found = RandomPoint(_currentTransform.position, 15f, out point);
                if (found)
                {
                    _destination = point;
                    _hasDestination = true;
                }
            }
            else
            {
                //Vector3 direction = (_destination - _currentTransform.position).normalized;
                enemy.MoveToNavMesh(_destination, patrolSpeed);
            }
        }
    }
    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        result = Vector3.zero;

        // 1. Obtener el área actual
        if (!NavMesh.SamplePosition(center, out NavMeshHit currentHit, 1.0f, NavMesh.AllAreas))
            return false;

        int currentArea = currentHit.mask;

        // 2. intentar encontrar un punto de la misma área
        for (int i = 0; i < 30; i++)   // 30 intentos de seguridad
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                // Evitar bordes → comprobar diferencia
                float edgeDistance = Vector3.Distance(hit.position, randomPoint);

                if (edgeDistance < 0.6f) // 0.6f = margen de seguridad
                {
                    result = hit.position;
                    return true;
                }
            }
        }

        return false;
    }
}
