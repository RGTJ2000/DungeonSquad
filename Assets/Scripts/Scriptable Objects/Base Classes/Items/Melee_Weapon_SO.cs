using UnityEngine;

[CreateAssetMenu(fileName = "Melee_Weapon_SO", menuName = "Item/Melee_Weapon_SO")]
public class Melee_Weapon_SO : Weapon_SO
{

    public float melee_reach;
    public float melee_damageBase;
    public float melee_damageRange;

    //Audio
    public string hitAudio_ID;
    public string missAudio_ID;



}
