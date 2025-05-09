using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory : MonoBehaviour
{
    // Currency
    public int copper_count = 0;
    public int silver_count = 0;
    public int gold_count = 0;
    public int platinum_count = 0;

    // Category-specific lists
    public List<RuntimeItem> rings = new List<RuntimeItem>();
    public List<RuntimeItem> helms = new List<RuntimeItem>();
    public List<RuntimeItem> amulets = new List<RuntimeItem>();

    public List<RuntimeItem> meleeWeapons = new List<RuntimeItem>();
    public List<RuntimeItem> armors = new List<RuntimeItem>();
    public List<RuntimeItem> rangedWeapons = new List<RuntimeItem>();

    public List<RuntimeItem> shields = new List<RuntimeItem>();
    public List<RuntimeItem> boots = new List<RuntimeItem>();
    public List<RuntimeItem> missiles = new List<RuntimeItem>();

    public List<RuntimeItem> potions = new List<RuntimeItem>();
    public List<RuntimeItem> scrolls = new List<RuntimeItem>();
    public List<RuntimeItem> gems = new List<RuntimeItem>();

    private CoinRelay _coinRelay;

    private void Start()
    {
        _coinRelay = GetComponent<CoinRelay>();
    }

    public void ReceiveCoin(CoinType coinType, int amount)
    {
        switch (coinType)
        {
            case CoinType.copper: copper_count += amount; break;
            case CoinType.silver: silver_count += amount; break;
            case CoinType.gold: gold_count += amount; break;
            case CoinType.platinum: platinum_count += amount; break;
        }

        if (_coinRelay != null)
        {
            _coinRelay.CoinReceived(coinType);
        }
    }

    public void AddItem(RuntimeItem item)
    {
        switch (item.category)
        {
            case ItemCategory.melee_weapon: meleeWeapons.Add(item); break;
            case ItemCategory.ranged_weapon: rangedWeapons.Add(item); break;
            case ItemCategory.missile: missiles.Add(item); break;
            case ItemCategory.armor: armors.Add(item); break;
            case ItemCategory.boots: boots.Add(item); break;
            case ItemCategory.shield: shields.Add(item); break;
            case ItemCategory.potion: potions.Add(item); break;
            case ItemCategory.scroll: scrolls.Add(item); break;
            case ItemCategory.amulet: amulets.Add(item); break;
            case ItemCategory.ring: rings.Add(item); break;
            case ItemCategory.helm: helms.Add(item); break;
            case ItemCategory.gem: gems.Add(item); break;
            default:
                Debug.LogWarning("Unknown item category: " + item.baseItem.item_name);
                break;
        }
    }

    public void Clear()
    {
        meleeWeapons.Clear();
        rangedWeapons.Clear();
        missiles.Clear();
        armors.Clear();
        shields.Clear();
        potions.Clear();
        scrolls.Clear();
        amulets.Clear();
        rings.Clear();
        gems.Clear();
        helms.Clear();
        boots.Clear();
    }

    // Read-only property that returns all items across categories
    public List<RuntimeItem> AllItems
    {
        get
        {
            List<RuntimeItem> all = new List<RuntimeItem>();
            all.AddRange(meleeWeapons);
            all.AddRange(rangedWeapons);
            all.AddRange(missiles);
            all.AddRange(armors);
            all.AddRange(shields);
            all.AddRange(potions);
            all.AddRange(scrolls);
            all.AddRange(amulets);
            all.AddRange(rings);
            all.AddRange(gems);
            all.AddRange(helms);
            all.AddRange(boots);
            return all;
        }
    }

    public void RemoveItem(RuntimeItem item)
    {
        switch (item.category)
        {

            case ItemCategory.melee_weapon:
                {
                    meleeWeapons.Remove(item);
                    break;
                }
            case ItemCategory.ranged_weapon:
                {
                    rangedWeapons.Remove(item);
                    break;
                }
            case ItemCategory.missile:
                {
                    missiles.Remove(item);
                    break;
                }
            case ItemCategory.ring:
                {
                    rings.Remove(item);
                    break;
                }
            case ItemCategory.helm:
                {
                    helms.Remove(item);
                    break;
                }
            case ItemCategory.amulet:
                {
                    amulets.Remove(item);
                    break;
                }
            case ItemCategory.shield:
                {
                    shields.Remove(item);
                    break;
                }
            case ItemCategory.boots:
                {
                    boots.Remove(item);
                    break;
                }
       

            default:
                {
                    Debug.LogWarning("Unknown item category to remove: " + item.baseItem.item_name);

                    break;
                }
        }
    }
}
