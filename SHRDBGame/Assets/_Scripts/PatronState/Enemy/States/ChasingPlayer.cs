using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;

public class ChasingPlayer : AEnemyState
{

    //atributos
    private Transform _currentTransform;
    private GameObject _player;
    [SerializeField] private float chaseSpeed;
    


    //Metodos
    public ChasingPlayer(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _player = enemy.PlayerAtSight();
        _currentTransform = enemy.GetGameObject().transform;
        //Debug.Log("Entering Chasing Player State");
    }

    public override void Exit()
    {
        
    }

    public override void FixedUpdate()
    {
        if (enemy.PlayerAtSight()!=null)
        {
            //Vector3 direction = ((Vector3)_player.transform.position - (Vector3)_currentTransform.position).normalized;
            enemy.MoveToNavMesh(_player.transform.position, enemy.GetChaseSpeed());
            enemy.LookAt(_player.transform.position);
        }
        else
        {
            enemy.SetState(new Patrol(enemy));
        }
    }

    public override void Update()
    {
        
    }
}
