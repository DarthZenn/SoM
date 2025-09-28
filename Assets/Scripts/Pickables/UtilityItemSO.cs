using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Utility Item", menuName = "Items/Utility Item")]
public class UtilityItemSO : ItemSO
{
    public enum UtilityItem { Lighter, Flashlight, Lantern}
    public UtilityItem utilityItem;
}
