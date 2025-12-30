using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class HijoChasing1 : AEnemyState
{
    private Transform _currentTransform;
    private GameObject _player;
    private float chaseSpeed;
    private float distanceToPlayer;
    private float _currentHealth;
    private float _maxHealth;
    private bool canRangeAttack = true;
    private int lastHealthStep = 0;
    private NavMeshAgent _agent;
    private Coroutine _cdCoroutine;
    public HijoChasing1(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _player = enemy.PlayerAtSight();
        _currentTransform = enemy.GetGameObject().transform;
        chaseSpeed = enemy.GetChaseSpeed();
        _currentHealth = enemy.GetCurrentHealth();
        _maxHealth = enemy.GetMaxHealth();
        _agent = enemy.GetNavMeshAgent();
        //Debug.Log("ENTERING CHASING STATE");
        //Debug.Log("Entering Chasing Player State");
        lastHealthStep = Mathf.FloorToInt((_maxHealth - _currentHealth) / (_maxHealth * 0.1f));
    }

    public override void Exit()
    {
        //Debug.Log("EXITING CHASING STATE");
        if (_cdCoroutine != null)
        {
            enemy.GetGameObject().GetComponent<MonoBehaviour>().StopCoroutine(_cdCoroutine); // Detiene solo esta, no todas
            _cdCoroutine = null;
        }
    }

    public override void FixedUpdate()
    {
        _currentHealth = enemy.GetCurrentHealth();
        int currentStep = Mathf.FloorToInt((_maxHealth - _currentHealth) / (_maxHealth * 0.1f));
        if (currentStep > lastHealthStep)
        {
            enemy.SetState(new HijoRange1(enemy));
            lastHealthStep = currentStep;
        }
        enemy.LookAt(_player.transform.position);
        float distanceToPlayer = Vector3.Distance(_currentTransform.position, _player.transform.position);
        enemy.GetNavMeshAgent().speed = chaseSpeed;
        enemy.GetNavMeshAgent().SetDestination(_player.transform.position);
        if (enemy.NAttacksRecieved() > 0)
        {
            if (canRangeAttack)
            {
                canRangeAttack = false;
                enemy.RangeAttackPlayer();
                enemy.PopAttack();
                _cdCoroutine = enemy.GetGameObject().GetComponent<MonoBehaviour>().StartCoroutine(CD());
            }
        }
        if (distanceToPlayer < 3f)
        {
            enemy.SetState(new HijoMelee1(enemy));
            return;
        }
    }

    public override void Update()
    {

    }
    private IEnumerator CD()
    {

        yield return new WaitForSeconds(1.5f);
        canRangeAttack = true;

    }

}
