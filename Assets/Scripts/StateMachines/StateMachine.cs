using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();
    public BaseState<EState> currentState { get; protected set; }

    protected bool isTransitioningState = false;

    // Start is called before the first frame update
    void Start()
    {
        currentState.EnterState();
    }

    // Update is called once per frame
    void Update()
    {
        EState nextState = currentState.GetNextState();

        if (!isTransitioningState && nextState.Equals(currentState.StateKey))
        {
            currentState.UpdateState();
        }
        else if (!isTransitioningState)
        {
            TransitionToState(nextState);
        }
    }

    public void TransitionToState(EState state)
    {
        isTransitioningState = true;
        currentState.ExitState();
        currentState = States[state];
        currentState.EnterState();
        isTransitioningState = false;
    }

    void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        currentState.OnTriggerStay(other);
    }

    void OnTriggerExit(Collider other)
    {
        currentState.OnTriggerExit(other);
    }
}
