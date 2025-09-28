using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MeleeWeaponHitbox : MonoBehaviour
{
    private ItemData itemData;
    private float damage;
    private GameObject enemy;

    private void Start()
    {
        itemData = gameObject.GetComponent<ItemData>();
        WeaponSO weapon = itemData.item as WeaponSO;
        damage = weapon.damage;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.name);

        if (other.CompareTag("Enemy"))
        {
            enemy = other.gameObject;
            //Debug.Log(enemy.name);
            enemy.GetComponent<EnemyStats>().DamageHealth(damage);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemy = null;
        }
    }
}
