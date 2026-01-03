using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class HijoFlash2 : AEnemyState
{
    private Transform _currentTransform;
    private GameObject _player;
    private float distanceToPlayer;
    private Coroutine _cdCoroutine;
    private NavMeshAgent _agent;
    public HijoFlash2(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _player = enemy.PlayerAtSight();
        _currentTransform = enemy.GetGameObject().transform;
        _agent = enemy.GetNavMeshAgent();
    }

    public override void Exit()
    {

    }

    public override void FixedUpdate()
    {

        enemy.Flashbang();
        enemy.SetState(new HijoChasing2(enemy));
    }

    public override void Update()
    {

    }
}
