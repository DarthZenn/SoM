using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EnemyState
{
    private float nextAttackTime = 0f;

    public EnemyAttack(EnemyContext context, EnemyStateMachine.EnemyState estate) : base(context, estate)
    {
        EnemyContext enemyContext = context;
    }

    public override void EnterState()
    {
        //Debug.Log("Enemy Entering Attack State");
        enemyContext.agent.isStopped = true;
        // Wait 2s before first attack
        nextAttackTime = Time.time /*+ (enemyContext.attackCooldown / 2f)*/;
    }

    public override void ExitState()
    {
        enemyContext.animator.SetBool("isAttacking", false);
    }

    public override void UpdateState()
    {
        // Check if cooldown has expired
        if (Time.time >= nextAttackTime)
        {
            // Face the player
            Vector3 direction = (enemyContext.playerTransform.position - enemyContext.agent.transform.position).normalized;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                enemyContext.agent.transform.rotation = Quaternion.LookRotation(direction);
            }

            // Trigger attack
            enemyContext.animator.SetBool("isAttacking", true);
            enemyContext.animator.Play("Z_Attack", 0, 0f);

            // Reset cooldown
            nextAttackTime = Time.time + enemyContext.attackCooldown;
        }
    }

    public override EnemyStateMachine.EnemyState GetNextState()
    {
        float distance = Vector3.Distance(enemyContext.agent.transform.position, enemyContext.playerTransform.position);
        float currentHP = enemyContext.enemyStats.GetCurrentHeath();

        if (currentHP <= 0)
        {
            return EnemyStateMachine.EnemyState.Die;
        }

        if (enemyContext.animator.GetBool("isAttacking") == true)
        {
            return StateKey;
        }
        else if (distance > enemyContext.attackRadius)
        {
            return EnemyStateMachine.EnemyState.Idle;
        }

        return StateKey;
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
