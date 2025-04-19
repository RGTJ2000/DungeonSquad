using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuntimeItem
{
    public Item_SO baseItem;
    public int stackCount;

    public List<Charm_SO> attachedCharms;


    // Constructor
    public RuntimeItem(Item_SO item)
    {
        baseItem = item;
        stackCount = 1;
        attachedCharms = new List<Charm_SO>();

        // Optionally copy any charms from the base SO as a starting point
        if (item.attachedCharms != null)
            attachedCharms.AddRange(item.attachedCharms);
    }

    public string item_name => baseItem.item_name;
    public string description => baseItem.description;
    public ItemCategory category => baseItem.category;
    public Sprite Icon => baseItem.item_icon;
    public GameObject item_prefab => baseItem.item_prefab;

    public bool IsStackable => baseItem.isStackable;
    public int maxStack => baseItem.maxStack;

    public string GetDisplayName()
    {
        // Could combine charm names, rarity colors, etc.
        return baseItem.item_name;
    }
}
