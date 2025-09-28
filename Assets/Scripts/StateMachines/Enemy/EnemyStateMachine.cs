using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class EnemyStateMachine : StateMachine<EnemyStateMachine.EnemyState>
{
    public enum EnemyState
    {
        Idle,
        Walk,
        Run,
        Die,
        Attack,
        Stagger,
    }

    private EnemyContext _context;

    [SerializeField] private Transform _enemyTransform;
    [SerializeField] NavMeshAgent _agent;
    [SerializeField] Animator _animator;
    [SerializeField] private float _detectionRadius; // configurable
    [SerializeField] private float _fovRadius;
    [SerializeField] private float _fovAngle; // configurable
    [SerializeField] private float _attackRadius; //configurable
    [SerializeField] private float _attackCooldown; //configurable
    [SerializeField] private EnemyStats _enemyStats;
    [SerializeField] private LayerMask _ignoreLayers; //configurable

    private Transform _playerTransform;

    void Awake()
    {
        ValidateConstraints();

        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _context = new EnemyContext(_agent, _animator, _detectionRadius, _fovRadius, _fovAngle, _playerTransform, _attackRadius, _attackCooldown, _enemyStats, _ignoreLayers);
        InitializeStates();
    }

    private void ValidateConstraints()
    {
        Assert.IsNotNull(_enemyTransform, "Transform used to Optimus Prime the enemy is not assigned");
        Assert.IsNotNull(_agent, "Nav Mesh Agent used to navigate enemy is not assigned");
        Assert.IsNotNull(_animator, "Animator used to control animations is not assigned");
        Assert.IsNotNull(_enemyStats, "EnemyStats used to manage enemy's stats is not assigned");
    }

    private void InitializeStates()
    {
        States.Add(EnemyState.Idle, new EnemyIdle(_context, EnemyState.Idle));
        States.Add(EnemyState.Walk, new EnemyWalk(_context, EnemyState.Walk));
        States.Add(EnemyState.Run, new EnemyRun(_context, EnemyState.Run));
        States.Add(EnemyState.Die, new EnemyDie(_context, EnemyState.Die));
        States.Add(EnemyState.Attack, new EnemyAttack(_context, EnemyState.Attack));
        States.Add(EnemyState.Stagger, new EnemyStagger(_context, EnemyState.Stagger));
        currentState = States[EnemyState.Idle];
    }

    private void OnDrawGizmosSelected()
    {
        if (_enemyTransform == null) return;

        // Draw proximity detection radius (red)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_enemyTransform.position, _detectionRadius);

        // Draw FOV radius (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_enemyTransform.position, _fovRadius);

        // Draw FOV angle (two lines)
        Vector3 forward = _enemyTransform.forward;
        Quaternion leftRotation = Quaternion.Euler(0, -_fovAngle * 0.5f, 0);
        Quaternion rightRotation = Quaternion.Euler(0, _fovAngle * 0.5f, 0);

        Vector3 leftDirection = leftRotation * forward;
        Vector3 rightDirection = rightRotation * forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_enemyTransform.position, _enemyTransform.position + leftDirection * _fovRadius);
        Gizmos.DrawLine(_enemyTransform.position, _enemyTransform.position + rightDirection * _fovRadius);

        // Draw Attack Radius (magenta)
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_enemyTransform.position, _attackRadius);
    }
}
