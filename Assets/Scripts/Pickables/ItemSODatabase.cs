using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Database/ItemSODatabase")]
public class ItemSODatabase : ScriptableObject
{
    public List<ItemSO> allItems;

    private Dictionary<string, ItemSO> itemDict;

    public void Init()
    {
        itemDict = new Dictionary<string, ItemSO>();
        foreach (var item in allItems)
        {
            if (!itemDict.ContainsKey(item.itemID))
                itemDict.Add(item.itemID, item);
        }
    }

    public ItemSO GetItem(string id)
    {
        if (itemDict == null) Init();
        return itemDict.ContainsKey(id) ? itemDict[id] : null;
    }
}