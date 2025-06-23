using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;


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

public enum ActionMode
{
    combat,
    item
}

public enum StatType
{
    strength,
    dexterity,
    intelligence,
    will,
    soul,
    maxHealth
}

#region COMBAT ENUMS STRUCTS

public enum DamageType
{
    physical,
    confusion,
    fear,
    fire,
    frost,
    poison,
    sleep
}

public enum CombatResultType
{
    hit,
    critical,
    miss,
    parry,
    block,
    dodge,
    resist
}

[System.Serializable]
public struct DamageStats
{
    public DamageType damageType;
    public float damage_base;
    public float damage_range;

    public DamageStats(DamageType damageType, float damage_base, float damage_range)
    {
        this.damageType = damageType;
        this.damage_base = damage_base;
        this.damage_range = damage_range;
    }
   
}

public struct DamageResult
{
    private DamageType damageType;
    private float damageAmount;

    public DamageResult(DamageType damageType, float damageAmount)
    {
        this.damageType = damageType;
        this.damageAmount = damageAmount;
       
    }

    public DamageType DamageType => damageType;
    public float DamageAmount => damageAmount;


}

public struct MissileData
{
    public GameObject attacker;
    public GameObject target;
    public float attacker_rangedAR;
    public float critChance;
    public List<DamageStats> damageList;

    public MissileData(GameObject attacker, GameObject target, float attacker_rangedAR, float critChance, List<DamageStats> damageList)
    {
        this.attacker = attacker;
        this.target = target;
        this.attacker_rangedAR = attacker_rangedAR;
        this.critChance = critChance;
        this.damageList = damageList;
    }

  

}

public struct CombatResult
{
    public GameObject attacker;
    public CombatResultType resultType;
    public List<DamageResult> damageResultList;

    public CombatResult(GameObject attacker, CombatResultType resultType, List<DamageResult> resultList)
    {
        this.attacker = attacker;
        this.resultType = resultType;
        this.damageResultList = resultList;
    }
}


#endregion