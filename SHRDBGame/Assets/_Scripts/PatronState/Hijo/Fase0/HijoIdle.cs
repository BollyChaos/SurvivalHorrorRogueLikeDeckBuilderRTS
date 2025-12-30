using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;

public class HijoIdle : AEnemyState
{
    public HijoIdle(IEnemy enemy) : base(enemy)
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
        if (enemy.PlayerAtSight()!=null)
        {
            enemy.SetState(new HijoChasing1(enemy));
            return;
        }
    }

    // Start is called before the first frame update
}
