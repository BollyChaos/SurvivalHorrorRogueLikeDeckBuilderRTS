using System.Collections;
using System.Collections.Generic;
using State.Interfaces;
using UnityEngine;

public class HijaMisions : AEnemyState
{
    //atributos
    
    //metodos
    public HijaMisions(IEnemy enemy) : base(enemy)
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
        if(enemy.AreMisionsCompleted())
        {
            enemy.SetState(new HijaWaiting(enemy));
        }
    }
}
