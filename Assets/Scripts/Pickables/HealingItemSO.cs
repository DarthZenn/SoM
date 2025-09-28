using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healing Item", menuName = "Items/Healing Item")]
public class HealingItemSO : ItemSO
{
    public enum HealType { Health, Sanity }
    public HealType healType;
    public float healPercentage;
}
