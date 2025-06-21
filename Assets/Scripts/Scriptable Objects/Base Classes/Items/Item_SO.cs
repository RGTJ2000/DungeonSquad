using NUnit.Framework;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

// Inventory categorization
public enum ItemCategory
{
    none,
    melee_weapon,
    ranged_weapon,
    missile,
    helm,
    armor,
    shield,
    boots,
    potion,
    scroll,
    amulet,
    ring,
    gem,
    chest

}

public enum OperatorType
{
    percent,
    additive
}

[System.Serializable]
public struct StatAdjustment
{
    public OperatorType operatorType;
    public float amount;
}

//[CreateAssetMenu(fileName = "Item_SO", menuName = "Item/Item_SO")]
public abstract class Item_SO : ScriptableObject
{

    // Basic metadata
    public string item_name = "New Item";
    [TextArea] public string description = "Item description";
    // Item Category
    public ItemCategory category;

    public Sprite item_icon;
    public GameObject item_prefab;
    public string pickupAudio_ID;
    public string dropAudio_ID;

    public string equipAudio_ID;
    public string unequipAudio_ID;


    //Equip behavior
    public bool isEquippable = true;


    // Stacking behavior
    public bool isStackable = false;
    public int maxStack = 1; // 1 = not stackable

    //Charms
    public List<Charm_SO> attachedCharms;

    //Stat Modifiers
    public StatAdjustment strModifier;
    public StatAdjustment dexModifier;
    public StatAdjustment intModifier;
    public StatAdjustment willModifier;
    public StatAdjustment soulModifier;

   

}
