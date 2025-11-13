using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class TioPatrolling : AEnemyState
{
    //atributos
    private Transform _currentTransform;
    private Vector3 _destination;
    private bool _hasDestination = false;
    private float patrolSpeed;
    private NavMeshAgent _agent;
    //Metodos
    public TioPatrolling(IEnemy enemy): base(enemy){}

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
       if(enemy.PlayerAtSight()!=null) //Si detecta al jugador
        {
            //Debug.Log("Player Spotted, switching to Chasing Player State");
            enemy.SetState(new TioBattling(enemy));
        }
       else
        {
            //Comportamiento de patrulla
            if(!_hasDestination||(_destination - _currentTransform.position).magnitude < 0.5f)
            {
                Vector3 point;
                bool found = RandomPoint(_currentTransform.position,15f,out point);
                if(found)
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
    private bool RandomPoint (Vector3 center,float range,out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range; //punto aleatorio dentro de una esfera
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint,out hit,1.0f,NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }
}
