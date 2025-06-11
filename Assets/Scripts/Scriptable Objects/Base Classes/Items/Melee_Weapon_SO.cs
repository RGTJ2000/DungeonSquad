using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Melee_Weapon_SO", menuName = "Item/Melee_Weapon_SO")]
public class Melee_Weapon_SO : Weapon_SO
{

    public float melee_reach;
    public float melee_damageBase;
    public float melee_damageRange;
    public float melee_critBase;
    public List<DamageStats> damageList;

    public float str_scalingFactor;
    public float dex_scalingFactor;

    public float defense_dexBonusFactor;

    public float attack_strBonusFactor;
    public float attack_dexBonusFactor;

    public float attack_intBonusFactor;
    public float attack_willBonusFactor;

    //Audio
    public string hitAudio_ID;
    public string missAudio_ID;



}
