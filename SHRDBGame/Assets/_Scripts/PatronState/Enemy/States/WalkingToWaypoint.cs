using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class WalkingToWaypoint : AEnemyState
{

    //atributos
    private Transform _currentTransform;
    private float _patrolSpeed;
    private Transform _currentWaypoint;
    //Metodos
    public WalkingToWaypoint(IEnemy enemy) : base(enemy)
    { 
    }

    public override void Enter()
    {
        _currentTransform = enemy.GetGameObject().transform;
        _patrolSpeed = enemy.GetPatrolSpeed();
        _currentWaypoint = enemy.GetCurrentWaypoint();
    }

    public override void Exit()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {   
        Vector3 distanceToWaypoint = _currentWaypoint.position - _currentTransform.position;
        if (distanceToWaypoint.magnitude < 0.5f)
        {
            enemy.NextWaypoint();
            _currentWaypoint = enemy.GetCurrentWaypoint();
        }
        else if (enemy.PlayerAtSight()!=null) //Si detecta al jugador
        {
            //Debug.Log("Player Spotted, switching to Chasing Player State");
            enemy.SetState(new ChasingPlayer(enemy));
        }
        else
        {
            Vector3 direction = ((Vector3)_currentWaypoint.position - (Vector3)_currentTransform.position).normalized;
            enemy.MoveTo(direction, _patrolSpeed);
            enemy.LookAt(_currentWaypoint.position);
        }
    }
}
