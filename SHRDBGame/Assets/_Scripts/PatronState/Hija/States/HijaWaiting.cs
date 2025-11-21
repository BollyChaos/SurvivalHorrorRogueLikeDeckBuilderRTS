using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;

public class HijaWaiting : AEnemyState
{
    public HijaWaiting(IEnemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        if(enemy.IsSalonAbierto()&& !enemy.AreMisionsCompleted())
        {
            enemy.SetState(new HijaMisions(enemy));
        }
    }
}
