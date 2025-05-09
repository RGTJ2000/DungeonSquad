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
    public bool IsEquippable => baseItem.isEquippable;
    public int maxStack => baseItem.maxStack;

    public string GetDisplayName()
    {
        // Could combine charm names, rarity colors, etc.
        return baseItem.item_name;
    }


    // --- Typed Accessors ---
    public Melee_Weapon_SO MeleeWeapon => baseItem as Melee_Weapon_SO;
    public Ranged_Weapon_SO RangedWeapon => baseItem as Ranged_Weapon_SO;
    public Missile_SO Missile => baseItem as Missile_SO;

    /*
    public Armor_SO Armor => baseItem as Armor_SO;
    public Shield_SO Shield => baseItem as Shield_SO;
    public Potion_SO Potion => baseItem as Potion_SO;
    public Scroll_SO Scroll => baseItem as Scroll_SO;
    public Amulet_SO Amulet => baseItem as Amulet_SO;
    public Ring_SO Ring => baseItem as Ring_SO;
    public Gem_SO Gem => baseItem as Gem_SO;
    */
}
