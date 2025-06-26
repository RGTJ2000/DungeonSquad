using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball Effect", menuName = "Spells/Fireball Effect")]
public class FireballEffect : Skill_SO
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

    public float castingTime;
    public Vector3 castingOffset;

    public override void Use(GameObject caster, GameObject target)
    {
        
        EntityStats _casterStats = caster.GetComponent<EntityStats>();
        SkillCooldownTracker _cooldownTracker = caster.GetComponent<SkillCooldownTracker>();

        Vector3 instantiatePoint = caster.transform.position + (caster.transform.forward * _casterStats.entity_radius) + Quaternion.LookRotation(caster.transform.forward, Vector3.up) * castingOffset;


        GameObject fireball = Instantiate(fireball_prefab, instantiatePoint, caster.transform.rotation);

        fireball.transform.localScale = Vector3.zero;

        Fireball_Guidance _fbGuidance = fireball.GetComponent<Fireball_Guidance>();

        float castingTime_adjusted = castingTime / (1 + StatScale(_casterStats.int_adjusted));

        _cooldownTracker.StartCooldown(this, castingTime_adjusted);
       
        _fbGuidance.SetParameters(caster, target, contactDamageStats, blastDamageStats, startSize, castingTime_adjusted, travelSpeed, blastDiameter, blastImpulse, blastSpeed, _casterStats.magic_attackRating);
        
    }

    private float StatScale(float stat)
    {
        return (stat - 50f) / 50f;
    }

}
