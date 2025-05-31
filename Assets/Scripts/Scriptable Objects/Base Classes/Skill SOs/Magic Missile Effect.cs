using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Magic Missile Effect", menuName = "Spells/Magic Missile Effect")]
public class MagicMissileEffect : SpellEffect_SO
{
    public GameObject magicMissile_prefab;

    public float damageBase;
    public float damageRange;

    public List<DamageStats> damageStats;
  

    public float acceleration = 200f;

    public override void Execute(Spell_SO spell, GameObject caster, GameObject target)
    {
        EntityStats _casterStats = caster.GetComponent<EntityStats>();

        Vector3 instantiatePoint = caster.transform.position + (caster.transform.forward * _casterStats.entity_radius) + Quaternion.LookRotation(caster.transform.forward, Vector3.up)* spell.castingOffsetfromCaster;

        //Debug.Log("Instantiating MM");
        GameObject magicMissile = Instantiate(magicMissile_prefab, instantiatePoint, caster.transform.rotation);

        CapsuleCollider mmCollider = magicMissile.GetComponent<CapsuleCollider>();
        CapsuleCollider entityCollider = caster.GetComponent<CapsuleCollider>();

        Physics.IgnoreCollision(mmCollider, entityCollider, true);


        MM_Guidance _mmGuidance2 = magicMissile.GetComponent<MM_Guidance>();

        _mmGuidance2.SetMMParameters(caster, target, spell.castingTime, acceleration, damageStats, spell.alwaysHit, _casterStats.magic_attackRating);

    }

 
}
