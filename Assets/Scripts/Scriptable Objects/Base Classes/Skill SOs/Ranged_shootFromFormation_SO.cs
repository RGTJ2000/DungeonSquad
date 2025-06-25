using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Ranged_shootFromFormation_SO", menuName = "Ranged Skills/Ranged_shootFromFormation_SO")]
public class Ranged_shootFromFormation_SO : Skill_SO
{
    public Skill_SO rangedAttack_SO;

    public override void Use(GameObject attacker, GameObject target)
    {
        if (target != null)
        {
            EntityStats _entityStats = attacker.GetComponent<EntityStats>();
            NavMeshAgent _navMeshAgent = attacker.GetComponent<NavMeshAgent>();
            SkillCooldownTracker _cooldownTracker = attacker.GetComponent<SkillCooldownTracker>();

            //MOVE ATTACKER WITH FORMATION
            Vector3 target_direction = (target.transform.position - attacker.transform.position).normalized;
            target_direction.y = 0;
            FaceTarget(attacker, target_direction);

            if (Vector3.Angle(attacker.transform.forward, target_direction) < 10f)
            {
                RaycastHit hit;
                if (Physics.Raycast(attacker.transform.position, target.transform.position - attacker.transform.position, out hit, _entityStats.visible_distance, ~0, QueryTriggerInteraction.Ignore) && hit.transform.gameObject.tag == "Enemy")
                {
                    Debug.Log("RangedAttack Cooldown = " + _cooldownTracker.GetRemainingCooldown(rangedAttack_SO));
                    if (_cooldownTracker != null && _cooldownTracker.GetRemainingCooldown(rangedAttack_SO) == 0)
                    {
                        //PERFORM ACTUAL RANGED ATTACK
                        if (rangedAttack_SO != null)
                        {
                            rangedAttack_SO.Use(attacker, target);
                        }
                    }


                }

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
