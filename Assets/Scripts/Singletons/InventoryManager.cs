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

    public List<RuntimeItem> GetCoreMeleeWeaponsList()
    {
        return _coreInventory.meleeWeapons;
    }

    public List <RuntimeItem> GetCoreRangedWeaponsList()
    {
        return _coreInventory.rangedWeapons;
    }

    public List<RuntimeItem> GetCoreMissilesList()
    {
        return _coreInventory.missiles;
    }

    public void DropItemFromCore (RuntimeItem item)
    {
        //instantiate dropped item
        DropManager.Instance.ThrowRuntimeItem(item, _core);
        //remove item from core
        _coreInventory.RemoveItem(item);

    }


}
