using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect_SO", menuName = "Incants/HealEffect_SO")]
public class HealEffect_SO : Skill_SO
{
    public float healAmount;
    public GameObject healing_prefab;
    public float healCycleTime;

    public override void Use(GameObject incanter, GameObject target)
    {
        

        SkillCooldownTracker _cooldownTracker = incanter.GetComponent<SkillCooldownTracker>();

        Health _targetHealth = target.GetComponent<Health>();

        if (_targetHealth.currentHealth == _targetHealth.maxHealth)
        {
            if (incanter.TryGetComponent<Ch_Behavior>(out var ch_behavior))
            {
                ch_behavior.CancelEngage();
                return;
            }

        }
        else if (_cooldownTracker.GetRemainingCooldown(this) == 0)
        {

            EntityStats _incanterStats = incanter.GetComponent<EntityStats>();

            float _cycleTime_adjusted = healCycleTime / (1 + StatScale(_incanterStats.soul_adjusted));

            GameObject healing_obj = Instantiate(healing_prefab, target.transform.position, Quaternion.identity);
            healing_obj.GetComponent<HealingObj_Behavior>().SetAndStartLifetime(target, _cycleTime_adjusted, healAmount);

            //_targetHealth.Heal(healAmount);
            _cooldownTracker.StartCooldown(this, _cycleTime_adjusted);

        }
        


    }


    private float StatScale(float stat)
    {
        return (stat - 50f) / 50f;
    }

}
