using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalk : EnemyState
{
    public EnemyWalk(EnemyContext context, EnemyStateMachine.EnemyState estate) : base(context, estate)
    {
        EnemyContext enemyContext = context;
    }

    public override void EnterState()
    {
        //Debug.Log("Enemy Entering Walk State");
        enemyContext.agent.isStopped = false;
        enemyContext.animator.SetBool("isWalking", true);
    }

    public override void ExitState()
    {
        enemyContext.agent.ResetPath();
        enemyContext.animator.SetBool("isWalking", false);
        enemyContext.animator.SetBool("isShot", false);
    }

    public override void UpdateState()
    {
        AnimatorStateInfo stateInfo = enemyContext.animator.GetCurrentAnimatorStateInfo(0);

        // If still attacking (animation not done yet)
        if (enemyContext.animator.GetBool("isAttacking") == true && stateInfo.IsTag("EnemyAttack"))
        {
            return;
        }

        enemyContext.agent.SetDestination(enemyContext.playerTransform.position);
    }

    public override EnemyStateMachine.EnemyState GetNextState()
    {
        float distance = Vector3.Distance(enemyContext.agent.transform.position, enemyContext.playerTransform.position);
        float currentHP = enemyContext.enemyStats.GetCurrentHeath();

        if (currentHP <= 0)
        {
            return EnemyStateMachine.EnemyState.Die;
        }

        // If close enough to attack, stop chasing and go Idle first
        if (distance <= enemyContext.attackRadius)
            return EnemyStateMachine.EnemyState.Idle;

        return StateKey; // lost player
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
