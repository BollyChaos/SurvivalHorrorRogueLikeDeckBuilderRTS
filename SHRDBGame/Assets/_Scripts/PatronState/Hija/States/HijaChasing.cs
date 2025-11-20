using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class HijaChasing : AEnemyState
{
    //atributos
    private Transform _currentTransform;
    private Vector3 _destination;
    private NavMeshAgent _agent;
    private float chaseSpeed;
    public HijaChasing(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        
    }
}
