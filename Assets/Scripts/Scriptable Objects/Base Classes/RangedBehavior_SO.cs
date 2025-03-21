using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "RangedBehavior_SO", menuName = "Enemy Behavior/RangedBehavior_SO")]
public class RangedBehavior_SO : EngageBehavior_SO
{

    public override void Perform(GameObject enemy_obj, ScanForCharacters _scanForCharacters, NavMeshAgent _navMeshAgent, EntityStats _entityStats)
    {
        GameObject target_obj = _scanForCharacters.targeted_character;
        if (target_obj != null)
        {
            

            Vector3 target_direction = (target_obj.transform.position - enemy_obj.transform.position).normalized;
            target_direction.y = 0;
            FaceTarget(enemy_obj, target_direction);

            
            RaycastHit hit;

            if (Physics.SphereCast(enemy_obj.transform.position, _entityStats.entity_radius, target_direction, out hit, _entityStats.visible_distance, ~0, QueryTriggerInteraction.Ignore) && hit.transform.gameObject.tag == "Character")
            {
                //if the enemy has a straight line to character then hold position
                _navMeshAgent.destination = enemy_obj.transform.position;
            }
            else
            {
                //if enemy has no straight line, then move towards character
                _navMeshAgent.destination = target_obj.transform.position;
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
