using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "MagicBehavior_MM_SO", menuName = "Enemy Behavior/MagicBehavior_MM_SO")]
public class MagicBehavior_SO : EngageBehavior_SO
{
    public override void Perform(GameObject attacker, GameObject target)
    {
        if (target != null)
        {
            EntityStats _entityStats = attacker.GetComponent<EntityStats>();
            NavMeshAgent _navMeshAgent = attacker.GetComponent<NavMeshAgent>();

            Vector3 target_direction = (target.transform.position - attacker.transform.position).normalized;
            target_direction.y = 0;

            FaceTarget(attacker, target_direction);

            int missilesLayer = 11;
            int layerMask = ~(1 << missilesLayer); // Exclude layer 11 from check for casting obstructions

            RaycastHit hit;

            if (Physics.SphereCast(attacker.transform.position, _entityStats.entity_radius, target_direction, out hit, _entityStats.visible_distance, layerMask, QueryTriggerInteraction.Ignore) && hit.transform.gameObject.tag == "Character")
            {
                //if the enemy has a straight line to character then hold position
                _navMeshAgent.destination = attacker.transform.position;
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
    
