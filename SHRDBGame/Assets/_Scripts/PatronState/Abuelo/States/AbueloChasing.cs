using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class AbueloChasing : AEnemyState
{
   
    //atributos
    private Transform _currentTransform;
    private Vector3 _destination;
    private NavMeshAgent _agent;
    private bool _isResting = false;
    private float _timeSinceLastRest = 0f;
    private float _restDuration;
    private float chaseSpeed;
    
    private Coroutine _restCoroutine; // referencia a la coroutine activa

    //Metodos
    public AbueloChasing(IEnemy enemy,Vector3 SoundPos) : base(enemy)
    {
        _destination = SoundPos;
    }

    public override void Enter()
    {
        _currentTransform = enemy.GetGameObject().transform;
        _agent = enemy.GetNavMeshAgent();
        _restDuration = enemy.GetRestDuration();
        chaseSpeed = enemy.GetChaseSpeed();
        //Debug.Log("Entering Chasing Player State");

        // Configurar agente
        _agent.speed = chaseSpeed;
        _agent.isStopped = false;
    }

    public override void Exit()
    {
        _agent.isStopped = false;
        if (_restCoroutine != null)
        {
            enemy.GetGameObject().GetComponent<MonoBehaviour>().StopCoroutine(_restCoroutine); // Detiene solo esta, no todas
            _restCoroutine = null;
        }
        _isResting = false;
    }

    public override void FixedUpdate()
    {
        if (enemy.PlayerAtSight()!=null)
        {
            enemy.SetState(new AbueloBattling(enemy));
            return;
        }
        if (_isResting) { return; }

        float distanceToSound = Vector3.Distance(_currentTransform.position, _destination);
        if (distanceToSound < 0.5f)
        {
            enemy.AttackPlayer();
            enemy.SetState(new AbueloPatrolling(enemy));
            //Vector3 direction = ((Vector3)_player.transform.position - (Vector3)_currentTransform.position).normalized;
        }
        else
        {
            enemy.MoveToNavMesh(_destination, enemy.GetChaseSpeed());

            // Control de tiempo entre descansos
            _timeSinceLastRest += Time.fixedDeltaTime;
            if (_timeSinceLastRest >= 2f) // cada 2 segundos descansa
            {
                enemy.GetGameObject().GetComponent<MonoBehaviour>().StartCoroutine(RestRoutine());
            }
        }
    }

    private IEnumerator RestRoutine()
    {
        _isResting = true;
        _timeSinceLastRest = 0f;

        _agent.isStopped = true;
        yield return new WaitForSeconds(_restDuration);
        _agent.isStopped = false;

        enemy.MoveToNavMesh(_destination, enemy.GetChaseSpeed());
        _isResting = false;
    }
    
    public override void Update()
    {

    }
}
