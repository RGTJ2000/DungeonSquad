using UnityEngine;

public enum weaponType { melee, ranged, missile};

[CreateAssetMenu(fileName = "Weapon_SO", menuName = "Weapon/Weapon_SO")]
public class Weapon_SO : ScriptableObject
{
    //UNIVERSAL STATS
    public string weaponName; //e.g. "Crude Iron Sword"
    public weaponType weaponType; // enum "melee", "ranged"
    public string hitAudio_variationID;
    public string missAudio_variationID;

    //MELLE AND RANGED STATS
    public float attackCooldown;

    public float critChance_base;           // 0-1 chance of crit

    public float critChance_weight_str;     // 0-1 weighting of how much this stat matters
    public float critChance_weight_dex;     //etc.
    public float critChance_weight_int;
    public float critChance_weight_will;
    public float critChance_weight_soul;

    //MELEE only stats
    public float melee_reach;
    public float melee_damageBase;
    public float melee_damageRange;

    //RANGED only STATS
    public float launch_impulse;    //0.5 will launch a 0.1 kg projectile at 5 m/s
    public float accuracy_factor;   //1 = same. 2 = doubles accuracy, reduces angle cone by 2.

    //MISSILE only stats
    public float missile_damageBase;
    public float missile_damageRange;
    public float missile_weight;
    public GameObject missile_prefab;

}
