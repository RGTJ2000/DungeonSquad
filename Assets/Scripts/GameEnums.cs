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
    area
}