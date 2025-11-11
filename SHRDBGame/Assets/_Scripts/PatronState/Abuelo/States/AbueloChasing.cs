using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;

public class AbueloChasing : AEnemyState
{
   
    //atributos
    private Transform _currentTransform;
    private Vector3 _destination;
    [SerializeField] private float chaseSpeed;
    


    //Metodos
    public AbueloChasing(IEnemy enemy,Vector3 SoundPos) : base(enemy)
    {
        _destination = SoundPos;
    }

    public override void Enter()
    {
        _currentTransform = enemy.GetGameObject().transform;
        //Debug.Log("Entering Chasing Player State");
    }

    public override void Exit()
    {
        
    }

    public override void FixedUpdate()
    {
        float distanceToSound = Vector3.Distance(_currentTransform.position, _destination);
        if (distanceToSound < 0.5f)
        {
            //Vector3 direction = ((Vector3)_player.transform.position - (Vector3)_currentTransform.position).normalized;
            enemy.MoveToNavMesh(_destination, enemy.GetChaseSpeed());
        }
        else
        {
            enemy.SetState(new AbueloPatrolling(enemy));
        }
    }

    public override void Update()
    {
        
    }
}
