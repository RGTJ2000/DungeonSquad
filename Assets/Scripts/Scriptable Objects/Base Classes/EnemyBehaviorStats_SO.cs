using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBehaviorStats_SO", menuName = "Enemy Behavior/EnemyBehaviorStats_SO")]
public class EnemyBehaviorStats_SO : ScriptableObject
{
    public float trigger_activeRadius;
    public float trigger_active_disengageRadius;

    public float trigger_farRadius;
    public float trigger_far_disengageRadius;
    public float trigger_nearRadius;

    public bool visualContact_required;

    public float trigger_lowHealth;

    public Entity_DefaultBehavior defaultBehavior;
    public DefaultBehavior_SO defaultBehavior_SO;
    public AlertBehavior_SO alertBehavior_SO;
    public EngageBehavior_SO engageBehavior_SO;

    public float wander_radius;
}
