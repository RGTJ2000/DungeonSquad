using UnityEngine;


public abstract class SpellEffect_SO : ScriptableObject
{
    
    public abstract void Execute(Spell_SO spell, GameObject caster, GameObject target);
}