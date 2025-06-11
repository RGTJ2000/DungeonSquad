using UnityEngine;

[CreateAssetMenu(fileName = "Ranged_Weapon_SO", menuName = "Item/Ranged_Weapon_SO")]
public class Ranged_Weapon_SO : Weapon_SO
{

    public float ranged_critBase;

    public float launch_impulse;    //0.5 will launch a 0.1 kg projectile at 5 m/s
    public float accuracy_factor;   //1 = same. 2 = doubles accuracy, reduces angle cone by 2.
    //public float ranged_critBase;

    //Audio
    public string launchAudio_ID;

    public float attack_dexBonusFactor;
    public float attack_intBonusFactor;

}
