using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Weapon")]
public class WeaponSO : ItemSO
{
    public enum WeaponCategory { Melee, Pistol, Shotgun, AssaultRifle }
    public WeaponCategory weaponCategory;
    public float damage;
    public bool isTwoHanded;

    [Header("Gun Settings")]
    public float fireRate;
    public int magazineSize;
    public ItemSO ammoType; // Reference to ammo SO
    public float range;       // effective shooting distance
    public float arcAngle; // arc range
}
