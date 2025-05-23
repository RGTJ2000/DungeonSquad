using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using static UnityEditor.Progress;

public static class ItemEvents
{
    public static event Action OnItemPickedUp;

    public static void RaiseItemPickedUp()
    {
        OnItemPickedUp?.Invoke();
    }
}

public class InventoryManager : ManagerBase<InventoryManager>
{
    private GameObject _core;
    private Inventory _coreInventory;

    protected override void Awake()
    {
        base.Awake();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //assign coreInventory
        _core = GameObject.FindWithTag("Core");
        _coreInventory = _core.GetComponent<Inventory>();
    }

    public List<RuntimeItem> GetCoreRingsList()
    {
        return _coreInventory.rings;
    }
    public List<RuntimeItem> GetCoreHelmsList()
    {
        return _coreInventory.helms;
    }
    public List<RuntimeItem> GetCoreAmuletsList()
    {
        return _coreInventory.amulets;
    }

    public List<RuntimeItem> GetCoreMeleeWeaponsList()
    {
        return _coreInventory.meleeWeapons;
    }
    public List<RuntimeItem> GetCoreArmorsList()
    {
        return _coreInventory.armors;
    }
    public List<RuntimeItem> GetCoreRangedWeaponsList()
    {
        return _coreInventory.rangedWeapons;
    }

    public List<RuntimeItem> GetCoreShieldsList()
    {
        return _coreInventory.shields;
    }
    public List<RuntimeItem> GetCoreBootsList()
    {
        return _coreInventory.boots;
    }
    public List<RuntimeItem> GetCoreMissilesList()
    {
        return _coreInventory.missiles;
    }

    public void DropItemFromCore(RuntimeItem item)
    {
        //instantiate dropped item

        DropManager.Instance.ThrowRuntimeItem(item, _core);
        //remove item from core
        _coreInventory.RemoveItem(item);

    }

    public void UnequipItemfromCharacter(RuntimeItem item, GameObject char_obj)
    {
        EntityStats _entityStats = char_obj.GetComponent<EntityStats>();

        //add item to Core
        _coreInventory.AddItem(item);

        //remove item from equipped
        switch (item.category)
        {
            case ItemCategory.ring:
                _entityStats.equipped_ring = null;
                break;

            case ItemCategory.helm:
                _entityStats.equipped_helm = null;
                break;

            case ItemCategory.amulet:
                _entityStats.equipped_amulet = null;
                break;

            case ItemCategory.melee_weapon:
                _entityStats.equipped_meleeWeapon = null;
                break;

            case ItemCategory.armor:
                _entityStats.equipped_armor = null;
                break;

            case ItemCategory.ranged_weapon:
                _entityStats.equipped_rangedWeapon = null;
                break;

            case ItemCategory.shield:
                _entityStats.equipped_shield = null;
                break;

            case ItemCategory.boots:
                _entityStats.equipped_boots = null;
                break;

            case ItemCategory.missile:
                _entityStats.equipped_missile = null;
                break;

            default:
                Debug.LogWarning("Unknown item category: " + item.category);
                break;



        }

        //Update character adjusted stats
        _entityStats.UpdateAdjustedStats();
    }

    public RuntimeItem EquipItemToCharacter(RuntimeItem item, GameObject char_obj)
    {
        EntityStats _entityStats = char_obj.GetComponent<EntityStats>();

        ItemCategory _categroyToEquip = item.category;

        RuntimeItem itemToUnequip = null;

        switch (_categroyToEquip)
        {
            case ItemCategory.ring:
                itemToUnequip = _entityStats.equipped_ring;
                _entityStats.equipped_ring = item;
                break;
            case ItemCategory.helm:
                itemToUnequip = _entityStats.equipped_helm;
                _entityStats.equipped_helm = item;
                break;
            case ItemCategory.amulet:
                itemToUnequip = _entityStats.equipped_amulet;
                _entityStats.equipped_amulet = item;
                break;
            case ItemCategory.melee_weapon:
                itemToUnequip = _entityStats.equipped_meleeWeapon;
                _entityStats.equipped_meleeWeapon = item;
                break;
            case ItemCategory.armor:
                itemToUnequip = _entityStats.equipped_armor;
                _entityStats.equipped_armor = item;
                break;
            case ItemCategory.ranged_weapon:
                itemToUnequip = _entityStats.equipped_rangedWeapon;
                _entityStats.equipped_rangedWeapon = item;
                break;
            case ItemCategory.shield:
                itemToUnequip = _entityStats.equipped_shield;
                _entityStats.equipped_shield = item;
                break;
            case ItemCategory.boots:
                itemToUnequip = _entityStats.equipped_boots;
                _entityStats.equipped_boots = item;
                break;
            case ItemCategory.missile:
                itemToUnequip = _entityStats.equipped_missile;
                _entityStats.equipped_missile = item;
                break;
            default:
                itemToUnequip = null;
                break;
        }


        
        _coreInventory.RemoveItem(item);
        //Debug.Log("Removed " + item.item_name + " from Core.");

        //Update character adjusted stats
        _entityStats.UpdateAdjustedStats();

        if (itemToUnequip != null)
        {
            _coreInventory.AddItem(itemToUnequip);
            //Debug.Log("Added "+itemToUnequip.item_name+" to Core.");
            return itemToUnequip;
        }
        else
        {
            return null;
        }


    }
}
