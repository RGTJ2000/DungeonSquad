using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Magic Missile Effect", menuName = "Spells/Magic Missile Effect")]
public class MagicMissileEffect : Skill_SO
{
    public GameObject magicMissile_prefab;

    public float damageBase;
    public float damageRange;
    public float castingTime;
    public Vector3 castingOffset;
    public bool alwaysHit = false;

    public List<DamageStats> damageStats;
  

    public float acceleration = 200f;

    public override void Use(GameObject attacker, GameObject target)
    {
        EntityStats _attackerStats = attacker.GetComponent<EntityStats>();
        SkillCooldownTracker _cooldownTracker = attacker.GetComponent<SkillCooldownTracker>();

        Vector3 instantiatePoint = attacker.transform.position + (attacker.transform.forward * _attackerStats.entity_radius) + Quaternion.LookRotation(attacker.transform.forward, Vector3.up)* castingOffset;

        //Debug.Log("Instantiating MM");
        GameObject magicMissile = Instantiate(magicMissile_prefab, instantiatePoint, attacker.transform.rotation);

        CapsuleCollider mmCollider = magicMissile.GetComponent<CapsuleCollider>();
        CapsuleCollider entityCollider = attacker.GetComponent<CapsuleCollider>();

        Physics.IgnoreCollision(mmCollider, entityCollider, true);


        MM_Guidance _mmGuidance2 = magicMissile.GetComponent<MM_Guidance>();

        float _castingTime_adjusted = castingTime / (1 + StatScale(_attackerStats.int_adjusted));
        _cooldownTracker.StartCooldown(this, _castingTime_adjusted);

        _mmGuidance2.SetMMParameters(attacker, target, _castingTime_adjusted, acceleration, damageStats, alwaysHit, _attackerStats.magic_attackRating);

    }

    private float StatScale(float stat)
    {
        return (stat - 50f) / 50f;
    }

}
