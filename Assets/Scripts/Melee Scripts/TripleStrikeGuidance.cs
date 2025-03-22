using UnityEngine;

public class TripleStrikeGuidance : MonoBehaviour
{
    private GameObject attacker;
    private GameObject target;
    private EntityStats _attackerStats;
    private SkillCooldownTracker _attackerCooldownTracker;

    [SerializeField] Skill_SO melee_multiHit_SO;

    private int hitCount;


    // Update is called once per frame
    void Update()
    {
        if (hitCount < 3)
        {
            if (_attackerCooldownTracker.GetRemainingCooldown(melee_multiHit_SO) == 0)
            {
                if (attacker != null && target != null)
                {
                    //Debug.Log("Multi strike #" + (hitCount + 1));
                    melee_multiHit_SO.Use(attacker, target);
                    //cooldown set by multiHit_SO
                    hitCount++;
                }
                
            }
        }
        else
        {
            Destroy(gameObject);
        }
        


    }

    public void SetTripleStrikeParameters(GameObject attacker_obj, GameObject target_obj)
    {
        attacker = attacker_obj;
        target = target_obj;
        _attackerCooldownTracker = attacker.GetComponent<SkillCooldownTracker>();
        hitCount = 0;
    }
}
