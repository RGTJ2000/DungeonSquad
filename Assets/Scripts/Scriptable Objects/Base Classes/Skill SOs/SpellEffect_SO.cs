using UnityEngine;


public abstract class SpellEffect_SO : ScriptableObject
{
    
    public abstract void Execute(CastSpell_SO spell, GameObject caster, GameObject target);
}