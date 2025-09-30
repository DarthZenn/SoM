using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1HMeleeCombat : PlayerState
{
    public Player1HMeleeCombat(PlayerContext context, PlayerStateMachine.PlayerState estate) : base(context, estate)
    {
        PlayerContext playerContext = context;
    }

    public override void EnterState()
    {
        //Debug.Log("Entering Melee Combat State");
        playerContext.animator.SetBool("is1HMeleeCombat", true);
    }

    public override void ExitState()
    {
        playerContext.animator.SetBool("is1HMeleeCombat", false);
    }

    public override void UpdateState()
    {
        AnimatorStateInfo stateInfo = playerContext.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("MeleeAttack") == true)
        {
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");

        playerContext.Rotate(horizontal);

        // Update animator parameter
        if (Input.GetMouseButtonDown(0))
        {
            playerContext.animator.SetBool("is1HMeleeAttack", true);
        }

        playerContext.animator.SetFloat("Horizontal", horizontal);
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        // --- Check for dying ---
        if (PlayerStats.Instance.currentHealth <= 0 || PlayerStats.Instance.currentSanity <= 0)
        {
            return PlayerStateMachine.PlayerState.Die;
        }

        // Leave combat if RMB released
        if (!Input.GetMouseButton(1))
        {
            return PlayerStateMachine.PlayerState.Idle;
        }

        return StateKey; // stay in melee combat
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
