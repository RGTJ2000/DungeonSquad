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


    public float defense_dexModifier;

    public float attack_strModifier;
    public float attack_dexModifier;

    public float attack_intModifier;
    public float attack_willModifer;

    //Audio
    public string hitAudio_ID;
    public string missAudio_ID;



}
