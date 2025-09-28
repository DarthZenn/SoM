using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    public EnemyStats enemyStats;
    //private GameObject player;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            /*player = other.gameObject;
            player.GetComponent<PlayerStats>().DamageHealth(enemyStats.GetAttackDamage());
            player.GetComponent<PlayerStats>().DamageSanity(enemyStats.GetSanityDamage());*/
            PlayerStats.Instance.DamageHealth(enemyStats.GetAttackDamage());
            PlayerStats.Instance.DamageSanity(enemyStats.GetSanityDamage());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //player = null;
        }
    }
}
