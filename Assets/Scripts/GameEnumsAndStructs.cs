using UnityEngine;


public enum Entity_DefaultBehavior
{
    idle,
    wander,
    patrol
}

public enum Enemy_EngageType
{
    blind,
    dumb,
    lazy,
    tenacious,
    unstoppable

}

public enum CombatType
{
    melee,
    ranged,
    magic,
    incant
}

public enum Targeting_Tag
{
    enemy,
    character
}

public enum Targeting_Type
{
    other,
    group,
    self,
    area,
    item
}

public enum Character_Type
{
    fighter,
    cleric,
    wizard,
    ranger
}

public struct ch_info
{
    public string ch_type;
    public int slot_number;
    public GameObject obj_ref;
    // Constructor
    public ch_info(string type, int slot, GameObject obj)
    {
        ch_type = type;
        slot_number = slot;
        obj_ref = obj;
    }
}

public enum CoinType
{
    copper,
    silver,
    gold,
    platinum
}

public enum EngageMode
{
    combat,
    item
}