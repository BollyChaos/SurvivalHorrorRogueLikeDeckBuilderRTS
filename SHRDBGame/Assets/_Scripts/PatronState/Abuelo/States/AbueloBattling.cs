using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class AbueloBattling : AEnemyState
{
   

    //atributos
    private Transform _currentTransform;
    private GameObject _player;
    private float chaseSpeed;
    private float distanceToPlayer;
    private NavMeshAgent _agent;

    private Coroutine _cdCoroutine; // referencia a la coroutine activa

    //Metodos
    public AbueloBattling(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _player = enemy.PlayerAtSight();
        _currentTransform = enemy.GetGameObject().transform;
        _agent = enemy.GetNavMeshAgent();
        chaseSpeed = enemy.GetChaseSpeed();
        //Debug.Log("ENTERING BATTLING STATE");
        //Debug.Log("Entering Chasing Player State");
    }

    public override void Exit()
    {
        //Debug.Log("EXITING BATTLING STATE");
        _agent.isStopped = false;
        if (_cdCoroutine != null)
        {
            enemy.GetGameObject().GetComponent<MonoBehaviour>().StopCoroutine(_cdCoroutine); // Detiene solo esta, no todas
            _cdCoroutine = null;
        }
    }

    public override void FixedUpdate()
    {
        enemy.LookAt(_player.transform.position); 
        float distanceToPlayer = Vector3.Distance(_currentTransform.position, _player.transform.position);
        if (distanceToPlayer < 3f&& _agent.isStopped == false)
        {
                   
                enemy.AttackPlayer();
                enemy.GetGameObject().GetComponent<MonoBehaviour>().StartCoroutine(CD());
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
private IEnumerator CD()
    {
        _agent.isStopped = true;
        Debug.Log("Abuelo attacking, cooldown started");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Abuelo attack cooldown ended");
        _agent.isStopped = false;

    }
}
