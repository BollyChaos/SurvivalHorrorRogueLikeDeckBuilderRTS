using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class VecinoBattling : AEnemyState
{
    //atributos
    private Transform _currentTransform;
    private GameObject _player;
    
    private NavMeshAgent _agent;
    private float chaseSpeed;

    //Metodos
    public VecinoBattling(IEnemy enemy) : base(enemy) { }

    public override void Enter()
    {
        _currentTransform = enemy.GetGameObject().transform;
        _agent = enemy.GetNavMeshAgent();
        chaseSpeed = enemy.GetChaseSpeed();
        _player = enemy.PlayerAtSight();
        //Debug.Log("Entering Chasing Player State");

        // Configurar agente
        _agent.speed = chaseSpeed;
        _agent.isStopped = false;
    }

    public override void Exit()
    {
        
    }

    public override void FixedUpdate()
    {
        
        enemy.LookAt(_player.transform.position); 
        float distanceToPlayer = Vector3.Distance(_currentTransform.position, _player.transform.position);
        if (distanceToPlayer < 2f&& _agent.isStopped == false)
        {
                   
                enemy.AttackPlayer();
                enemy.GetGameObject().GetComponent<MonoBehaviour>().StartCoroutine(CD());
            }
            else if (distanceToPlayer >= 8f)
            {
                enemy.NullPlayerAtSight();
                enemy.SetState(new VecinoPatrolling(enemy));
                return;
            }
            //Vector3 direction = ((Vector3)_player.transform.position - (Vector3)_currentTransform.position).normalized;
            enemy.MoveToNavMesh(_player.transform.position, _agent.speed);
    }

    public override void Update()
    {

    }
    private IEnumerator CD()
    {
        _agent.isStopped = true;
        //Debug.Log("Abuelo attacking, cooldown started");
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("Abuelo attack cooldown ended");
        _agent.isStopped = false;

    }
    
}
