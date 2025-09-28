using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo", menuName = "Items/Ammo")]
public class AmmoSO : ItemSO
{
    public enum AmmoCategory { PistolAmmo, SGAmmo, ARAmmo }
    public AmmoCategory ammoCategory;
}
