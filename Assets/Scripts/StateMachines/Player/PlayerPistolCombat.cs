using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPistolCombat : PlayerState
{
    private float nextFireTime = 0f; // when the player can fire again

    public PlayerPistolCombat(PlayerContext context, PlayerStateMachine.PlayerState estate) : base(context, estate)
    {
        PlayerContext playerContext = context;
    }

    public override void EnterState()
    {
        //Debug.Log("Entering Pistol Combat State");
        playerContext.animator.SetBool("isPistolCombat", true);
    }

    public override void ExitState()
    {
        playerContext.animator.SetBool("isPistolCombat", false);
        playerContext.animator.SetLayerWeight(1, 0f);
    }

    public override void UpdateState()
    {
        AnimatorStateInfo stateInfo = playerContext.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("GunReload") == true)
        {
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");

        // Rotation
        playerContext.Rotate(horizontal);

        // Update animator parameter
        playerContext.animator.SetFloat("Horizontal", horizontal);
        playerContext.animator.SetLayerWeight(1, horizontal != 0 ? 1f : 0f);

        // Shooting
        if (Input.GetMouseButtonDown(0)) // LMB
        {
            var equipSlot = InventoryManager.Instance.equipmentSlots[0]; // right hand slot
            if (equipSlot.itemData != null && equipSlot.itemData is WeaponSO weapon
                && weapon.weaponCategory == WeaponSO.WeaponCategory.Pistol)
            {
                if (Time.time >= nextFireTime) // cooldown check
                {
                    if (equipSlot.currentAmmo > 0)
                    {
                        // Force replay of shooting animation immediately
                        playerContext.animator.Play("PistolShoot", 0, 0f);
                        //Debug.Log("Bang! Pistol fired!");

                        // Update ammo
                        InventoryManager.Instance.GunShotUpdate();
                        equipSlot.CurrentGunAmmoUpdate();

                        // Pass weapon directly
                        playerContext.ShootGun(weapon);

                        // Cooldown
                        nextFireTime = Time.time + (1f / weapon.fireRate);
                    }
                    else
                    {
                        AudioManager.Instance.PlaySFXOneShot(Sound.PistolEmpty);
                        //Debug.Log("Click! No ammo.");
                        nextFireTime = Time.time + 0.2f; // tiny delay for dry fire
                    }
                }
            }
        }

        if (Input.GetButtonDown("Reload"))
        {
            AudioManager.Instance.PlaySFXOneShot(Sound.PistolReload); // Placeholder. Should be in the reload animation.
            playerContext.ReloadGun();
        }
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

        return StateKey; // stay in pistol combat
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
