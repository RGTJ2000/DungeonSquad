using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball Effect", menuName = "Spells/Fireball Effect")]
public class FireballEffect : SpellEffect_SO
{
    public GameObject fireball_prefab;

    public float startSize;
    public float travelSpeed;
    public float contact_damageBase;
    public float contact_damageRange;
    public List<DamageStats> contactDamageStats;

    public float blastDiameter;
    public float blastImpulse;
    public float blastSpeed;
    public float blast_damageBase;
    public float blast_damageRange;
    public List<DamageStats> blastDamageStats;

    public override void Execute(Spell_SO spell, GameObject caster, GameObject target)
    {
        EntityStats _casterStats = caster.GetComponent<EntityStats>();

        Vector3 instantiatePoint = caster.transform.position + (caster.transform.forward * _casterStats.entity_radius) + Quaternion.LookRotation(caster.transform.forward, Vector3.up) * spell.castingOffsetfromCaster;


        GameObject fireball = Instantiate(fireball_prefab, instantiatePoint, caster.transform.rotation);

        fireball.transform.localScale = Vector3.zero;

        Fireball_Guidance _fbGuidance = fireball.GetComponent<Fireball_Guidance>();

        _fbGuidance.SetParameters(caster, target, contactDamageStats, blastDamageStats, startSize, spell.castingTime, travelSpeed, blastDiameter, blastImpulse, blastSpeed, _casterStats.magic_attackRating);


    }


}
