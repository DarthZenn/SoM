using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerStateMachine : StateMachine<PlayerStateMachine.PlayerState>
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Die,
        MeleeCombat1H,
        PistolCombat,
    }

    private PlayerContext _context;

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _oneHandedMeleeHolder;
    [SerializeField] private GameObject _pistolHolder;
    [SerializeField] private LayerMask _ignoreLayers;

    void Awake()
    {
        ValidateConstraints();

        _context = new PlayerContext(_characterController, _moveSpeed, _rotateSpeed, _gravity, _animator, _oneHandedMeleeHolder, _pistolHolder, _ignoreLayers);
        InitializeStates();
    }

    private void ValidateConstraints()
    {
        Assert.IsNotNull(_characterController, "CharacterController used to control character is not assigned");
        Assert.IsNotNull(_animator, "Animator used to control animation is not assigned");
        Assert.IsNotNull(_oneHandedMeleeHolder, "Empty object used to hold melee weapon is not assigned");
        Assert.IsNotNull(_oneHandedMeleeHolder, "Empty object used to hold pistol is not assigned");
    }

    private void InitializeStates()
    {
        States.Add(PlayerState.Idle, new PlayerIdle(_context, PlayerState.Idle));
        States.Add(PlayerState.Walk, new PlayerWalk(_context, PlayerState.Walk));
        States.Add(PlayerState.Die, new PlayerDie(_context, PlayerState.Die));
        States.Add(PlayerState.MeleeCombat1H, new Player1HMeleeCombat(_context, PlayerState.MeleeCombat1H));
        States.Add(PlayerState.PistolCombat, new PlayerPistolCombat(_context, PlayerState.PistolCombat));
        currentState = States[PlayerState.Idle];
    }
}
