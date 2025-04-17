using UnityEngine;

[CreateAssetMenu(fileName = "New Single Shot", menuName = "Ranged Skills/SingleShot_SO")]
public class Ranged_SO : Skill_SO
{
    public override void Use(GameObject attacker, GameObject target)
    {
        
        MissileLauncher _missileLauncher = attacker.GetComponent<MissileLauncher>();
        _missileLauncher.LaunchMissile(target, attacker);

        
    }

}
    

