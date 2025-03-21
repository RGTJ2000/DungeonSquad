using UnityEngine;

[CreateAssetMenu(fileName = "Fireball Effect", menuName = "Spells/Fireball Effect")]
public class FireballEffect : SpellEffect_SO
{
    public GameObject fireball_prefab;

    public float startSize;
    public float travelSpeed;
    public float contact_damageBase;
    public float contact_damageRange;

    public float blastDiameter;
    public float blastImpulse;
    public float blastSpeed;
    public float blast_damageBase;
    public float blast_damageRange;

    public override void Execute(Spell_SO spell, GameObject caster, GameObject target)
    {
        EntityStats _casterStats = caster.GetComponent<EntityStats>();

        Vector3 instantiatePoint = caster.transform.position + (caster.transform.forward * _casterStats.entity_radius) + Quaternion.LookRotation(caster.transform.forward, Vector3.up) * spell.castingOffsetfromCaster;


        GameObject fireball = Instantiate(fireball_prefab, instantiatePoint, caster.transform.rotation);

        fireball.transform.localScale = Vector3.zero;

        Fireball_Guidance _fbGuidance = fireball.GetComponent<Fireball_Guidance>();

        _fbGuidance.SetParameters(caster, target, startSize, spell.castingTime, travelSpeed, contact_damageBase, contact_damageRange, blastDiameter, blastImpulse, blastSpeed, blast_damageBase, blast_damageRange, _casterStats.magic_attackRating);


    }


}
