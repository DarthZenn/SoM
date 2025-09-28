using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : PlayerState
{
    public PlayerWalk(PlayerContext context, PlayerStateMachine.PlayerState estate) : base(context, estate)
    {
        PlayerContext playerContext = context;
    }

    public override void EnterState()
    {
        //Debug.Log("Entering Walk State");
        playerContext.animator.SetBool("isWalking", true); // switch to walk animation
    }

    public override void ExitState()
    {
        playerContext.animator.SetBool("isWalking", false); // reset when leaving walk
    }

    public override void UpdateState()
    {
        AnimatorStateInfo stateInfo = playerContext.animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("MeleeAttack") == true || stateInfo.IsTag("GunReload") == true)
        {
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Rotation
        playerContext.Rotate(horizontal);

        // Movement
        Vector3 moveDirection = playerContext.characterController.transform.forward * vertical * playerContext.moveSpeed;
        playerContext.Move(moveDirection);

        // Update animator parameter
        playerContext.animator.SetFloat("Vertical", vertical);
        playerContext.animator.SetFloat("Horizontal", horizontal);
    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        // --- Check for dying ---
        if (PlayerStats.Instance.currentHealth <= 0 || PlayerStats.Instance.currentSanity <= 0)
        {
            return PlayerStateMachine.PlayerState.Die;
        }

        // --- Check for melee combat ---
        var equipSlot = InventoryManager.Instance.equipmentSlots[0];
        WeaponSO weapon = equipSlot.itemData as WeaponSO;

        if (equipSlot.itemData != null && Input.GetMouseButton(1))
        {
            if (weapon.weaponCategory == WeaponSO.WeaponCategory.Melee && weapon.isTwoHanded == false)
            {
                return PlayerStateMachine.PlayerState.MeleeCombat1H;
            }
            else if (weapon.weaponCategory == WeaponSO.WeaponCategory.Pistol)
            {
                return PlayerStateMachine.PlayerState.PistolCombat;
            }
        }

        // --- Check for movement ---
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(vertical) > 0.1f || Mathf.Abs(horizontal) > 0.1f)
            return StateKey; // stay walking

        return PlayerStateMachine.PlayerState.Idle; // stop = idle
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
