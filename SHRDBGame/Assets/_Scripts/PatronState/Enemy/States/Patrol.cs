using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : AEnemyState
{
    //atributos
    private Transform _currentTransform;
    private Vector3 _destination;
    private bool _hasDestination = false;
    private float patrolSpeed;
    //Metodos
    public Patrol(IEnemy enemy): base(enemy){Debug.Log("Patrol constructor called");}

    public override void Enter()
    {
        _currentTransform = enemy.GetGameObject().transform;
        _destination = _currentTransform.position;
        Debug.Log("distancia en el enter:"+(_destination - _currentTransform.position).magnitude);

        //Debug.Log("Entering Patrol State");
        _hasDestination = false;
        patrolSpeed = enemy.GetPatrolSpeed();
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
            enemy.SetState(new ChasingPlayer(enemy));
        }
       else
        {
            //lo comento porque peta un poco : )
            //Debug.Log("distancia en el update:"+(_destination - _currentTransform.position).magnitude);
            //Comportamiento de patrulla
            if(!_hasDestination||(_destination - _currentTransform.position).magnitude < 0.5f)
            {
                Vector3 point;
                bool found = RandomPoint(_currentTransform.position,10f,out point);
                if(found)
                {
                    Debug.Log("Este es el punto"+ point);
                    _destination = point;
                    _hasDestination = true;
                }
            }
            else
            {
                //Vector3 direction = (_destination - _currentTransform.position).normalized;
                enemy.MoveToNavMesh(_destination, patrolSpeed);
                enemy.LookAt(_destination);
                Debug.Log("Moviendose hacia el punto"+ _destination);
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
