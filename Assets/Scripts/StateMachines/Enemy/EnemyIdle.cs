using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdle : EnemyState
{
    public EnemyIdle(EnemyContext context, EnemyStateMachine.EnemyState estate) : base(context, estate)
    {
        EnemyContext enemyContext = context;
    }

    public override void EnterState()
    {
        //Debug.Log("Enemy Entering Idle State");
        enemyContext.agent.enabled = false;
    }

    public override void ExitState()
    {
        enemyContext.agent.enabled = true;
    }

    public override void UpdateState()
    {
        
    }

    public override EnemyStateMachine.EnemyState GetNextState()
    {
        float distance = Vector3.Distance(enemyContext.agent.transform.position, enemyContext.playerTransform.position);
        Vector3 directionToPlayer = (enemyContext.playerTransform.position - enemyContext.agent.transform.position).normalized;
        float currentHP = enemyContext.enemyStats.GetCurrentHeath();

        if (currentHP <= 0)
        {
            return EnemyStateMachine.EnemyState.Die;
        }

        if (distance <= enemyContext.attackRadius)
            return EnemyStateMachine.EnemyState.Attack;

        if (enemyContext.animator.GetBool("isShot") == true)
        {
            return EnemyStateMachine.EnemyState.Walk;
        }

        // Proximity check (with LoS)
        if (distance <= enemyContext.detectionRadius)
        {
            if (enemyContext.CheckIfPlayerIsInLineOfSight(directionToPlayer, enemyContext.detectionRadius))
                return EnemyStateMachine.EnemyState.Walk;
        }

        // FOV check (with LoS)
        if (distance <= enemyContext.fovRadius)
        {
            float angle = Vector3.Angle(enemyContext.agent.transform.forward, directionToPlayer);

            if (angle < enemyContext.fovAngle * 0.5f)
            {
                if (enemyContext.CheckIfPlayerIsInLineOfSight(directionToPlayer, enemyContext.fovRadius))
                    return EnemyStateMachine.EnemyState.Walk;
            }
        }

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
