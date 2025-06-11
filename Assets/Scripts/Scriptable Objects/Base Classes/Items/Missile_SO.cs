using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Missile_SO", menuName = "Item/Missile_SO")]
public class Missile_SO : Item_SO
{
    public float missile_damageBase;
    public float missile_damageRange;
    public float missile_critBaseModifier;
    public List<DamageStats> damageList;
    public int TestInteger;

    public float missile_weight;
    public GameObject missileLaunch_prefab;

    public float attack_dexBonusFactor;
    public float attack_intBonusFactor;

}
