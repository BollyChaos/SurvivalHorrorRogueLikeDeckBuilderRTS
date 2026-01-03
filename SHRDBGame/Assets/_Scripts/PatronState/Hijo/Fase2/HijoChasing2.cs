using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;
using UnityEngine.AI;

public class HijoChasing2 : AEnemyState
{
    private Transform _currentTransform;
    private GameObject _player;
    private float chaseSpeed;
    private float distanceToPlayer;
    private float _currentHealth;
    private float _maxHealth;
    private int lastHealthStep = 0;
    private NavMeshAgent _agent;
    public HijoChasing2(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        _player = enemy.GetPlayer();
        _currentTransform = enemy.GetGameObject().transform;
        chaseSpeed = enemy.GetChaseSpeed();
        _currentHealth = enemy.GetCurrentHealth();
        _maxHealth = enemy.GetMaxHealth();
        _agent = enemy.GetNavMeshAgent();
        //Debug.Log("ENTERING CHASING STATE");
        //Debug.Log("Entering Chasing Player State");
        lastHealthStep = Mathf.FloorToInt((_maxHealth - _currentHealth) / (_maxHealth * 0.15f));


        enemy.GetNavMeshAgent().speed = chaseSpeed;
    }

    public override void Exit()
    {
        //Debug.Log("EXITING CHASING STATE");
    }

    public override void FixedUpdate()
    {
        enemy.GetNavMeshAgent().SetDestination(_player.transform.position);
        _currentHealth = enemy.GetCurrentHealth();
        int currentStep = Mathf.FloorToInt((_maxHealth - _currentHealth) / (_maxHealth * 0.15f));
        if (currentStep > lastHealthStep)
        {
            if (enemy.NAttacksRecieved() % 2 == 0)
            {
                enemy.SetState(new HijoFlash2(enemy));
                lastHealthStep = currentStep;
            }
            else
            {
                enemy.SetState(new HijoFlash2(enemy));
                lastHealthStep = currentStep;
            }
        }
        enemy.LookAt(_player.transform.position);
        float distanceToPlayer = Vector3.Distance(_currentTransform.position, _player.transform.position);

        //Comprobar si puede hacer ataque a distancia viendo si ha recibido ataques

        if (distanceToPlayer < 3f)
        {
            enemy.SetState(new HijoMelee2(enemy));
            return;
        }
    }

    public override void Update()
    {

    }

}
