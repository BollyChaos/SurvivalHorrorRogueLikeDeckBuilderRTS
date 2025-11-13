using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class TioBattling : AEnemyState
{
    //atributos
    private Transform _currentTransform;
    private GameObject _player;
    private Vector3 _destination;
    private NavMeshAgent _agent;
    private float chaseSpeed;
    private bool _seenByPlayer = false;
    
    //Metodos
    public TioBattling(IEnemy enemy) : base(enemy) { }

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
        if (SeenByPlayer())
        {
            _agent.speed = chaseSpeed / 2f;
        }
        else
        {
            _agent.speed = chaseSpeed;
        }
        
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
    private bool SeenByPlayer()
    {
        if (_player == null) return false;

        // Vector desde el jugador hacia el enemigo
        Vector3 directionToEnemy = (_currentTransform.position - _player.transform.position).normalized;
        
        // Distancia entre jugador y enemigo
        float distanceToEnemy = Vector3.Distance(_player.transform.position, _currentTransform.position);

        // Comprobar distancia máxima
        if (distanceToEnemy > 10f)
            return false;

        // Ángulo entre la dirección forward del jugador y la dirección hacia el enemigo
        float angle = Vector3.Angle(_player.transform.forward, directionToEnemy);

        // Comprobar si está dentro del cono (30 grados para cada lado = 60 grados totales)
        if (angle <= 30f)
            return true;

        return false;
    }
}
