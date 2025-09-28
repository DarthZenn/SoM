using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyState : BaseState<EnemyStateMachine.EnemyState>
{
    protected EnemyContext enemyContext;

    public EnemyState(EnemyContext context, EnemyStateMachine.EnemyState stateKey) : base(stateKey)
    {
        enemyContext = context;
    }
}
