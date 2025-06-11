using UnityEngine;

[CreateAssetMenu(fileName = "Melee_singleHit_SO", menuName = "Melee Skills/Melee_singleHit_SO")]
public class Melee_singleHit_SO : Skill_SO
{
    EntityStats _entityStats;
    RuntimeItem equippedWeapon_SO;
    SkillCooldownTracker _cooldownTracker;

    public override void Use(GameObject user, GameObject target)
    {
        _entityStats = user.GetComponent<EntityStats>();
        _cooldownTracker = user.GetComponent<SkillCooldownTracker>();

        equippedWeapon_SO = _entityStats.equipped_meleeWeapon;

        cooldown = equippedWeapon_SO.MeleeWeapon.cycleTime / (1+ StatScale(_entityStats.dex_adjusted)) ; //set the cooldown to atacker's weapon

        CombatManager.Instance.ResolveMelee(user, target);

        if(_cooldownTracker != null)
        {
            _cooldownTracker.StartCooldown(this); //set the cooldown tracker

        }



    }

    private float StatScale(float stat)
    {
        return (stat - 50f) / 50f;
    }


}
    

