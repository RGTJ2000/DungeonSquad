using UnityEngine;

[CreateAssetMenu(fileName = "New Single Shot", menuName = "Ranged Skills/SingleShot_SO")]
public class Ranged_singleShot_SO : Skill_SO
{

    EntityStats _entityStats;
    RuntimeItem equippedRangedWeapon_SO;
    SkillCooldownTracker _cooldownTracker;

    private float _cooldown;

    public override void Use(GameObject attacker, GameObject target)
    {
        _entityStats = attacker.GetComponent<EntityStats>();
        _cooldownTracker = attacker.GetComponent<SkillCooldownTracker>();

        equippedRangedWeapon_SO = _entityStats.equipped_rangedWeapon;

        _cooldown = equippedRangedWeapon_SO.RangedWeapon.cycleTime / (1 + StatScale(_entityStats.dex_adjusted)); //set the cooldown to atacker's weapon

        MissileLauncher _missileLauncher = attacker.GetComponent<MissileLauncher>();
        _missileLauncher.LaunchMissile(target, attacker);

        if (_cooldownTracker != null)
        {
            Debug.Log("Ranged_singleShot_SO setting cooldown at " + _cooldown);
            _cooldownTracker.StartCooldown(this, _cooldown); //set the cooldown tracker for weapon cycle time

        }


    }

    private float StatScale(float stat)
    {
        return (stat - 50f) / 50f;
    }

}
    

