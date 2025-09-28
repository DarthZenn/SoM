using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerContext
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;
    [SerializeField] private float _gravity;
    [SerializeField] private Vector3 _velocity;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _oneHandedMeleeHolder;
    [SerializeField] private GameObject _pistolHolder;
    [SerializeField] private LayerMask _ignoreLayers;
    [SerializeField] private Collider[] _overlapResults = new Collider[100];

    public PlayerContext(CharacterController characterController, float moveSpeed, float rotateSpeed,
        float gravity, Animator animator, GameObject oneHandedMeleeHolder, GameObject pistolHolder, LayerMask ignoreLayers)
    {
        _characterController = characterController;
        _moveSpeed = moveSpeed;
        _rotateSpeed = rotateSpeed;
        _gravity = gravity;
        _animator = animator;
        _oneHandedMeleeHolder = oneHandedMeleeHolder;
        _pistolHolder = pistolHolder;
        _ignoreLayers = ignoreLayers;
    }

    public void Rotate(float horizontalInput)
    {
        float rotation = horizontalInput * _rotateSpeed * Time.deltaTime;
        _characterController.transform.Rotate(0f, rotation, 0f);
    }

    public void Move(Vector3 moveDirection)
    {
        // Gravity
        if (_characterController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // stick to ground
        }
        _velocity.y += _gravity * Time.deltaTime;

        // Apply movement + gravity
        Vector3 move = (moveDirection + new Vector3(0, _velocity.y, 0)) * Time.deltaTime;
        _characterController.Move(move);
    }

    public void ReloadGun()
    {
        var equipSlot = InventoryManager.Instance.equipmentSlots[0]; // right-hand weapon slot
        if (equipSlot.itemData == null || !(equipSlot.itemData is WeaponSO weapon))
        {
            Debug.Log("No weapon equipped to reload.");
            return;
        }

        // Determine what ammo type this weapon uses
        AmmoSO.AmmoCategory neededAmmo;
        switch (weapon.weaponCategory)
        {
            case WeaponSO.WeaponCategory.Pistol:
                neededAmmo = AmmoSO.AmmoCategory.PistolAmmo;
                break;
            case WeaponSO.WeaponCategory.Shotgun:
                neededAmmo = AmmoSO.AmmoCategory.SGAmmo;
                break;
            case WeaponSO.WeaponCategory.AssaultRifle:
                neededAmmo = AmmoSO.AmmoCategory.ARAmmo;
                break;
            default:
                Debug.Log("This weapon cannot be reloaded.");
                return;
        }

        int ammoNeeded = weapon.magazineSize - equipSlot.currentAmmo;
        if (ammoNeeded <= 0)
        {
            Debug.Log("Magazine already full.");
            return;
        }

        // Ask InventoryManager for ammo
        int ammoGiven = InventoryManager.Instance.SearchAmmo(neededAmmo, ammoNeeded);

        if (ammoGiven > 0)
        {
            equipSlot.currentAmmo += ammoGiven;
            equipSlot.CurrentGunAmmoUpdate(); // update UI
            Debug.Log($"Reloaded {ammoGiven} rounds. Current Ammo: {equipSlot.currentAmmo}");
        }
        else
        {
            Debug.Log("No ammo available in inventory!");
        }
    }

    public void ShootGun(WeaponSO weapon)
    {
        Vector3 origin = _characterController.transform.position; // in the middle height
        Vector3 forward = _characterController.transform.forward;

        int hitCount = Physics.OverlapSphereNonAlloc(origin, weapon.range, _overlapResults, ~_ignoreLayers);

        /*// --- VISUALIZATION: cone + lines to targets (lasting 0.25s) ---
        Debug.DrawRay(origin, forward * weapon.range, Color.white, 0.25f); // forward
        Vector3 left = Quaternion.AngleAxis(-weapon.arcAngle, Vector3.up) * forward;
        Vector3 right = Quaternion.AngleAxis(weapon.arcAngle, Vector3.up) * forward;
        Debug.DrawRay(origin, left * weapon.range, Color.yellow, 0.25f);   // left cone edge
        Debug.DrawRay(origin, right * weapon.range, Color.yellow, 0.25f);  // right cone edge

        // draw a few arc segments to make the cone edge visible
        int segments = 16;
        for (int i = 0; i < segments; i++)
        {
            float t0 = -weapon.arcAngle + (weapon.arcAngle * 2f) * (i / (float)segments);
            float t1 = -weapon.arcAngle + (weapon.arcAngle * 2f) * ((i + 1) / (float)segments);
            Vector3 d0 = Quaternion.AngleAxis(t0, Vector3.up) * forward;
            Vector3 d1 = Quaternion.AngleAxis(t1, Vector3.up) * forward;
            Debug.DrawLine(origin + d0 * weapon.range, origin + d1 * weapon.range, Color.yellow, 0.25f);
        }

        // lines to every collider found; green if inside cone, gray if outside
        foreach (var hit in hits)
        {
            Vector3 targetPos = hit.ClosestPoint(origin);
            Vector3 toTarget = (targetPos - origin).normalized;
            float angle = Vector3.Angle(forward, toTarget);
            Color c = angle <= weapon.arcAngle ? Color.green : Color.gray;
            Debug.DrawLine(origin, targetPos, c, 0.25f);
        }*/

        // If pistol -> pick nearest enemy
        if (weapon.weaponCategory == WeaponSO.WeaponCategory.Pistol)
        {
            EnemyStats closestEnemy = null;
            float closestDist = Mathf.Infinity;

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = _overlapResults[i];
                if (hit == null || !hit.CompareTag("Enemy"))
                    continue;

                EnemyStats enemy = hit.GetComponent<EnemyStats>();
                if (enemy == null)
                    continue;

                Vector3 toTarget = (hit.transform.position - origin).normalized;
                float angle = Vector3.Angle(forward, toTarget);

                if (angle <= weapon.arcAngle)
                {
                    float dist = Vector3.Distance(origin, hit.transform.position);

                    // Line of sight check
                    if (Physics.Raycast(origin + Vector3.up, toTarget, out RaycastHit rayHit, weapon.range, ~_ignoreLayers))
                    {
                        if (rayHit.collider == hit && dist < closestDist)
                        {
                            closestDist = dist;
                            closestEnemy = enemy;
                        }
                    }
                }
            }

            if (closestEnemy != null)
            {
                Animator enemyAnimator = closestEnemy.GetComponent<Animator>();
                enemyAnimator.SetBool("isShot", true);
                closestEnemy.DamageHealth(weapon.damage);
                //Debug.Log($"Hit {closestEnemy.name} for {weapon.damage} damage!");
            }
            else
            {
                //Debug.Log("No enemy hit.");
                return;
            }
        }
        else if (weapon.weaponCategory == WeaponSO.WeaponCategory.Shotgun)
        {
            bool hitSomething = false;

            for (int i = 0; i < hitCount; i++)
            {
                Collider hit = _overlapResults[i];
                if (hit == null) continue;

                EnemyStats enemy = hit.GetComponent<EnemyStats>();
                if (enemy == null) continue;

                Vector3 toTarget = (hit.transform.position - origin).normalized;
                float angle = Vector3.Angle(forward, toTarget);

                if (angle <= weapon.arcAngle)
                {
                    if (Physics.Raycast(origin, toTarget, out RaycastHit rayHit, weapon.range, ~_ignoreLayers))
                    {
                        if (rayHit.collider == hit)
                        {
                            enemy.DamageHealth(weapon.damage);
                            //Debug.Log($"Shotgun hit {enemy.name} for {weapon.damage} damage!");
                            hitSomething = true;
                        }
                    }
                }
            }

            if (!hitSomething)
            {
                Debug.Log("Shotgun blast hit nothing.");
            }
        }
    }

    public CharacterController characterController => _characterController;
    public float moveSpeed => _moveSpeed;
    public float rotateSpeed => _rotateSpeed;
    public float gravity => _gravity;
    public Animator animator => _animator;
    public GameObject oneHandedMeleeHolder => _oneHandedMeleeHolder;
    public GameObject pistolHolder => _pistolHolder;
    public LayerMask ignoreLayers => _ignoreLayers;

    public Collider[] overlapResults => _overlapResults;
}
