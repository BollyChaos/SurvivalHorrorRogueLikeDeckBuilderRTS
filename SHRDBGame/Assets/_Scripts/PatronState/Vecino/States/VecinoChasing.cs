using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class VecinoChasing : AEnemyState
{
    //atributos
    private Transform _currentTransform;
    private Vector3 _destination;
    private NavMeshAgent _agent;
    private float chaseSpeed;
    
    private Coroutine _restCoroutine; // referencia a la coroutine activa

    //Metodos
    public VecinoChasing(IEnemy enemy,Vector3 SoundPos) : base(enemy)
    {
        _destination = SoundPos;
    }

    public override void Enter()
    {
        _currentTransform = enemy.GetGameObject().transform;
        _agent = enemy.GetNavMeshAgent();
        chaseSpeed = enemy.GetChaseSpeed();
        //Debug.Log("Entering Chasing Player State");

        // Configurar agente
        _agent.speed = chaseSpeed;
        _agent.isStopped = false;
    }

    public override void Exit()
    {
        _agent.isStopped = false;
    }

    public override void FixedUpdate()
    {
        if (enemy.PlayerAtSight()!=null)
        {
            enemy.SetState(new VecinoBattling(enemy));
            return;
        }

        float distanceToSound = Vector3.Distance(_currentTransform.position, _destination);
        if (distanceToSound < 0.5f)
        {
            enemy.SetState(new VecinoPatrolling(enemy));
            //Vector3 direction = ((Vector3)_player.transform.position - (Vector3)_currentTransform.position).normalized;
        }
        else
        {
            enemy.MoveToNavMesh(_destination, enemy.GetChaseSpeed());
        }
    }

    
    public override void Update()
    {

    }
}
