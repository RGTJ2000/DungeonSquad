using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StartingItemData
{
    public Item_SO baseItem;
    public int stackCount = 1;
    public List<Charm_SO> attachedCharms;
}
