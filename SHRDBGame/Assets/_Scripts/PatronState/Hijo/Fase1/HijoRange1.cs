using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class HijoRange1 : AEnemyState
{
    private Transform _currentTransform;
    private GameObject _player;
    private float distanceToPlayer;
    private Coroutine _cdCoroutine;
    private NavMeshAgent _agent;
    public HijoRange1(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _player = enemy.PlayerAtSight();
        _currentTransform = enemy.GetGameObject().transform;
        _agent = enemy.GetNavMeshAgent();

        _cdCoroutine = enemy.GetGameObject().GetComponent<MonoBehaviour>().StartCoroutine(CD());
        //meleeSpeed = enemy.GetMeleeSpeed();
        //Debug.Log("ENTERING MELEE STATE");
        //Debug.Log("Entering Melee State");
        
        
    }

    public override void Exit()
    {
        //Debug.Log("EXITING MELEE STATE");
        if (_cdCoroutine != null)
        {
            enemy.GetGameObject().GetComponent<MonoBehaviour>().StopCoroutine(_cdCoroutine); // Detiene solo esta, no todas
            _cdCoroutine = null;
        }
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        
    }
    private IEnumerator CD()
    {
        _agent.isStopped = true;
        yield return new WaitForSeconds(0.5f);
        enemy.RangeAttackPlayer();
        yield return new WaitForSeconds(0.7f);
        enemy.RangeAttackPlayer();
        yield return new WaitForSeconds(0.5f);
        _agent.isStopped = false;
        enemy.SetState(new HijoChasing1(enemy));

    }
}
