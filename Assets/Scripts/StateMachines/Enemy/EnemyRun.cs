using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRun : EnemyState
{
    public EnemyRun(EnemyContext context, EnemyStateMachine.EnemyState estate) : base(context, estate)
    {
        EnemyContext enemyContext = context;
    }

    public override void EnterState()
    {

    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }

    public override EnemyStateMachine.EnemyState GetNextState()
    {
        return StateKey; // stay idle
    }

    public override void OnTriggerEnter(Collider collider)
    {

    }

    public override void OnTriggerStay(Collider collider)
    {

    }

    public override void OnTriggerExit(Collider collider)
    {

    }
}
