using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class HijoChasing3 : AEnemyState
{
    private Transform _currentTransform;
    private GameObject _player;
    private float chaseSpeed;
    private float distanceToPlayer;

    private NavMeshAgent _agent;
    public HijoChasing3(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _player = enemy.GetPlayer();
        _currentTransform = enemy.GetGameObject().transform;
        chaseSpeed = enemy.GetChaseSpeed() + 1;
        _agent = enemy.GetNavMeshAgent();
        //Debug.Log("ENTERING CHASING STATE");
        //Debug.Log("Entering Chasing Player State");


        enemy.GetNavMeshAgent().speed = chaseSpeed;
    }

    public override void Exit()
    {
        //Debug.Log("EXITING CHASING STATE");
    }

    public override void FixedUpdate()
    {
        enemy.GetNavMeshAgent().SetDestination(_player.transform.position);
        enemy.LookAt(_player.transform.position);
        float distanceToPlayer = Vector3.Distance(_currentTransform.position, _player.transform.position);

        //Comprobar si puede hacer ataque a distancia viendo si ha recibido ataques

        if (distanceToPlayer < 4f)
        {
            enemy.SetState(new HijoMelee3(enemy));
            return;
        }
    }

    public override void Update()
    {

    }

}
