using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Healing,
    Weapon,
    Ammo
}

public abstract class ItemSO : ScriptableObject
{
    public string itemID; // Unique ID for saving/loading
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public string description;
    public int maxStack;
    public GameObject inventoryPrefab;
    public GameObject worldPrefab;
}
