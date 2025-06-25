using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "EngageBehavior_SO", menuName = "Enemy Behavior/EngageBehavior_SO")]
public abstract class EngageBehavior_SO : ScriptableObject
{
    public CombatType combatType;
    public Skill_SO skill_SO;


    public abstract void Perform(GameObject attacker, GameObject target);
}
