using UnityEngine;

public abstract class Weapon_SO : Item_SO
{
    public float attackCooldown;

    [Header("Crit Settings")]
    public float critChance_base;

    [Header("Stat Weights")]
    public float critChance_weight_str;
    public float critChance_weight_dex;
    public float critChance_weight_int;
    public float critChance_weight_will;
    public float critChance_weight_soul;
}
