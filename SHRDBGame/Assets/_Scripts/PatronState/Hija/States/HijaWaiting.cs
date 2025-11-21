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
        Debug.Log("Enter State HijaWaiting");
    }

    public override void Exit()
    {
        
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        if(enemy.GetGameObject().GetComponent<EnemyCombat>().stats.CurrentHealth < enemy.GetGameObject().GetComponent<EnemyCombat>().stats.MaxHealth)
        {
            enemy.SetState(new HijaBattling(enemy));
            return;
        }
        if(enemy.IsSalonAbierto()&& !enemy.AreMisionsCompleted())
        {
            enemy.SetState(new HijaMisions(enemy));
            return;
        }
    }
}
