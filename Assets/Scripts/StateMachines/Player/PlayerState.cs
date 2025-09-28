using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState : BaseState<PlayerStateMachine.PlayerState>
{
    protected PlayerContext playerContext;

    public PlayerState(PlayerContext context, PlayerStateMachine.PlayerState stateKey) : base(stateKey)
    {
        playerContext = context;
    }
}
