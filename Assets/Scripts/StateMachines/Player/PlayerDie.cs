using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDie : PlayerState
{
    public PlayerDie(PlayerContext context, PlayerStateMachine.PlayerState estate) : base(context, estate)
    {
        PlayerContext playerContext = context;
    }

    public override void EnterState()
    {
        Debug.Log("Entering Die State");
        playerContext.animator.SetBool("isDead", true);
    }

    public override void ExitState()
    {
        playerContext.animator.SetBool("isDead", false);
    }

    public override void UpdateState()
    {
        
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        // --- Check for respawn ---
        if (PlayerStats.Instance.currentHealth > 0 && PlayerStats.Instance.currentSanity > 0)
        {
            return PlayerStateMachine.PlayerState.Idle;
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
