using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;

public class AbueloBattling : AEnemyState
{
   

    //atributos
    private Transform _currentTransform;
    private GameObject _player;
    private float chaseSpeed;
    private float distanceToPlayer;


    //Metodos
    public AbueloBattling(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _player = enemy.PlayerAtSight();
        _currentTransform = enemy.GetGameObject().transform;
        //Debug.Log("ENTERING BATTLING STATE");
        //Debug.Log("Entering Chasing Player State");
    }

    public override void Exit()
    {
        //Debug.Log("EXITING BATTLING STATE");
    }

    public override void FixedUpdate()
    {
        float distanceToPlayer = Vector3.Distance(_currentTransform.position, _player.transform.position);
        if (distanceToPlayer < 1.5f)
            {
                enemy.AttackPlayer();
            }
            else if (distanceToPlayer >= 8f)
            {
                enemy.NullPlayerAtSight();
                enemy.SetState(new AbueloPatrolling(enemy));
                return;
            }
            //Vector3 direction = ((Vector3)_player.transform.position - (Vector3)_currentTransform.position).normalized;
            enemy.MoveToNavMesh(_player.transform.position, enemy.GetChaseSpeed());
    }

    public override void Update()
    {
        
    }

}
