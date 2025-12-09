using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class HijaBattling : AEnemyState
{
    //atributos
    private Transform _currentTransform;
    private GameObject _player;
    private NavMeshAgent _agent;
    private float chaseSpeed;
    private Coroutine _attackCooldown;
    private bool _canAttack = true;

    public HijaBattling(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _currentTransform = enemy.GetGameObject().transform;
        _agent = enemy.GetNavMeshAgent();
        chaseSpeed = enemy.GetChaseSpeed();
        _player = enemy.GetPlayer();
        enemy.SetTalkable(false);
        _agent.isStopped = false;
        _canAttack = true;
        enemy.SetTalkable(false);
        Debug.Log("Enter State HijaBattling");
    }

    public override void Exit()
    {
        _agent.isStopped = false;
        if (_attackCooldown != null)
        {
            enemy.GetGameObject().GetComponent<MonoBehaviour>().StopCoroutine(_attackCooldown);
            _attackCooldown = null;
        }
    }

    public override void FixedUpdate()
    {
        // if (_player == null)
        // {
        //     enemy.NullPlayerAtSight();
        //     enemy.SetState(new HijaPatrolling(enemy));
        //     return;
        // }

        float distanceToPlayer = Vector3.Distance(_currentTransform.position, _player.transform.position);

        // Ataque cuerpo a cuerpo (< 3m)
        if (distanceToPlayer < 3f && _canAttack)
        {
            _agent.isStopped = true;
            enemy.LookAt(_player.transform.position);
            enemy.AttackPlayer();
            _attackCooldown = enemy.GetGameObject().GetComponent<MonoBehaviour>().StartCoroutine(AttackCooldown(2f));
        }
        // Ataque a distancia (5m - 10m)
        else if (distanceToPlayer >= 5f && distanceToPlayer <= 10f && _canAttack)
        {
            _agent.isStopped = true;
            enemy.LookAt(_player.transform.position);
            enemy.RangeAttackPlayer();
            _attackCooldown = enemy.GetGameObject().GetComponent<MonoBehaviour>().StartCoroutine(AttackCooldown(2f));
        }
        // Perseguir al jugador (< 5m o > 10m)
        else if (distanceToPlayer < 5f || distanceToPlayer > 10f)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_player.transform.position);
            enemy.LookAt(_player.transform.position);
        }
        // Fuera de rango de visión
        else if (distanceToPlayer > 15f)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_player.transform.position);
        }
    }

    public override void Update()
    {
        
    }

    private IEnumerator AttackCooldown(float cooldownTime)
    {
        _canAttack = false;
        yield return new WaitForSeconds(cooldownTime);
        _canAttack = true;
        _attackCooldown = null;
    }
}