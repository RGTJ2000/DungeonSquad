using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "RangedBehavior_SO", menuName = "Enemy Behavior/RangedBehavior_SO")]
public class RangedSingleShotPursuit_SO : EngageBehavior_SO
{

    public override void Perform(GameObject attacker, GameObject target) 
    {
        if (target != null)
        {
            EntityStats _entityStats = attacker.GetComponent<EntityStats>();
            NavMeshAgent _navMeshAgent = attacker.GetComponent<NavMeshAgent>();
            SkillCooldownTracker _cooldownTracker = attacker.GetComponent<SkillCooldownTracker>();

            //MOVE ATTACKER. HOLD POSITION IF STRAIGHT LINE VIEW. IF NOT CLOSE DISTANCE.

            Vector3 target_direction = (target.transform.position - attacker.transform.position).normalized;
            target_direction.y = 0;
            FaceTarget(attacker, target_direction);

            
            RaycastHit hit;

            if (Physics.SphereCast(attacker.transform.position, _entityStats.entity_radius, target_direction, out hit, _entityStats.visible_distance, ~0, QueryTriggerInteraction.Ignore) && hit.transform.gameObject.tag == "Character")
            {
                //if the enemy has a straight line to character then hold position
                _navMeshAgent.destination = attacker.transform.position;

                if (_cooldownTracker != null && _cooldownTracker.GetRemainingCooldown(skill_SO) == 0)
                {
                    //PERFORM ACTUAL RANGED ATTACK
                    if (skill_SO != null)
                    {
                        skill_SO.Use(attacker, target);
                    }
                }



            }
            else
            {
                //if enemy has no straight line, then move towards character
                _navMeshAgent.destination = target.transform.position;
            }

        }





    }

    private void FaceTarget(GameObject origin_obj, Vector3 target_direction)
    {
        

        if (target_direction != Vector3.zero) // Avoid errors if direction is zero
        {
            Quaternion targetRotation = Quaternion.LookRotation(target_direction);
            origin_obj.transform.rotation = Quaternion.Slerp(origin_obj.transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }
}
