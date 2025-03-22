using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/Spell_SO")]

public class Spell_SO : Skill_SO
{
    [Header("General Stats")]
    public float castingTime;
    public Vector3 castingOffsetfromCaster;
    public float hitChanceMultiplier;

    [Header("Spell Effect")]
    [SerializeField] private SpellEffect_SO effect;

    public override void Use(GameObject caster, GameObject target)
    {
         if (effect != null)
            {
                effect.Execute(this, caster, target);
            }
            else
            {
                Debug.LogWarning($"Spell '{skill_name}' has no effect assigned!");
            }
    }
}