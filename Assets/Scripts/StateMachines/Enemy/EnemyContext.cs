using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class EnemyContext
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _fovRadius;
    [SerializeField] private float _fovAngle;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private EnemyStats _enemyStats;
    [SerializeField] private LayerMask _ignoreLayers;

    public EnemyContext(NavMeshAgent agent, Animator animator,
        float detectionRadius, float fovRadius, float fovAngle, Transform playerTransform, float attackRadius, float attackCooldown, EnemyStats enemyStats, LayerMask ignoreLayers)
    {
        _agent = agent;
        _animator = animator;
        _detectionRadius = detectionRadius;
        _fovRadius = fovRadius;
        _fovAngle = fovAngle;
        _playerTransform = playerTransform;
        _attackRadius = attackRadius;
        _attackCooldown = attackCooldown;
        _enemyStats = enemyStats;
        _ignoreLayers = ignoreLayers;
    }

    public bool CheckIfPlayerIsInLineOfSight(Vector3 dirToPlayer, float radius)
    {
        RaycastHit hit;

        if (Physics.Raycast(_agent.transform.position + Vector3.up, dirToPlayer, out hit, radius, ~ignoreLayers))
        {
            Debug.DrawRay(_agent.transform.position + Vector3.up, dirToPlayer, Color.red);
            //Debug.Log("hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Player"))
            {
                return true; // Player is visible
            }
        }
        return false;
    }

    public NavMeshAgent agent => _agent;
    public Animator animator => _animator;
    public float detectionRadius => _detectionRadius;
    public float fovRadius => _fovRadius;
    public float fovAngle => _fovAngle;
    public Transform playerTransform => _playerTransform;
    public float attackRadius => _attackRadius;
    public float attackCooldown => _attackCooldown;
    public EnemyStats enemyStats => _enemyStats;
    public LayerMask ignoreLayers => _ignoreLayers;
}
