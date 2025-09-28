using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventsManager : MonoBehaviour
{
    [Header("Player")] 
    public Animator playerAnimator;
    public GameObject playerOneHandedMeleeHolder;

    [Header("Enemy")]
    public Animator enemyAnimator;
    public GameObject enemyRHHitbox;
    public GameObject enemyLHHitbox;

    public void PlayPlayerFootstepSFX()
    {
        AudioManager.Instance.PlaySFXOneShot(Sound.PlayerFootstep);
    }

    public void PlayPistolShotSFX()
    {
        AudioManager.Instance.PlaySFXOneShot(Sound.PistolShot);
    }

    public void PlayPistolReloadSFX()
    {
        AudioManager.Instance.PlaySFXOneShot(Sound.PistolReload);
    }

    public void OpenGameOverMenu()
    {
        Time.timeScale = 0;
        GameManager.Instance.gameOverMenuOpen = true;
        GameManager.Instance.gameOverMenu.SetActive(true);
    }

    public void Enable1HMeleeAttackCollider()
    {
        playerOneHandedMeleeHolder.GetComponentInChildren<CapsuleCollider>().enabled = true;
    }

    public void Disable1HMeleeAttackCollider()
    {

        playerOneHandedMeleeHolder.GetComponentInChildren<CapsuleCollider>().enabled = false;
    }

    public void End1HMeleeAttack()
    {
        playerAnimator.SetBool("is1HMeleeAttack", false);
    }

    public void EnableEnemyAttackHitbox()
    {
        enemyRHHitbox.GetComponent<SphereCollider>().enabled = true;
        enemyLHHitbox.GetComponent<SphereCollider>().enabled = true;
    }

    public void DisableEnemyAttackHitbox()
    {
        enemyRHHitbox.GetComponent<SphereCollider>().enabled = false;
        enemyLHHitbox.GetComponent<SphereCollider>().enabled = false;
    }

    public void EndEnemyAttack()
    {
        enemyAnimator.SetBool("isAttacking", false);

        enemyRHHitbox.GetComponent<SphereCollider>().enabled = false;
        enemyLHHitbox.GetComponent<SphereCollider>().enabled = false;
    }
}
