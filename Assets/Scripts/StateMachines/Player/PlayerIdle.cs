using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : PlayerState
{
    public PlayerIdle(PlayerContext context, PlayerStateMachine.PlayerState estate) : base(context, estate)
    {
        PlayerContext playerContext = context;
    }

    public override void EnterState()
    {
        //Debug.Log("Entering Idle State");
        playerContext.animator.SetBool("isWalking", false); // switch to idle animation
        playerContext.animator.SetFloat("Vertical", 0f);
        playerContext.animator.SetFloat("Horizontal", 0f);
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }

    public override PlayerStateMachine.PlayerState GetNextState()
    {
        // --- Check for dying ---
        if (PlayerStats.Instance.currentHealth <= 0 || PlayerStats.Instance.currentSanity <= 0)
        {
            return PlayerStateMachine.PlayerState.Die;
        }

        // --- Check for combat ---
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

        // --- Check for walking ---
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(vertical) > 0.1f || Mathf.Abs(horizontal) > 0.1f)
            if (!GameManager.Instance.mainMenuOpen)
                return PlayerStateMachine.PlayerState.Walk;

        return StateKey; // stay idle
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
