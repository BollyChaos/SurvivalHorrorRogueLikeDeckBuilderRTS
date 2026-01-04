using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;

public class HijoFire2 : AEnemyState
{
    public HijoFire2(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        enemy.FireAttack();
        enemy.SetState(new HijoChasing2(enemy));
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
