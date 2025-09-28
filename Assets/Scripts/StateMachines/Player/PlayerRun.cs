using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : PlayerState
{
    public Run(PlayerContext context, PlayerStateMachine.PlayerState estate) : base(context, estate)
    {
        PlayerContext playerContext = context;
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

    public override PlayerStateMachine.PlayerState GetNextState()
    {
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
