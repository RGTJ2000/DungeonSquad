using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "MeleePursuit_SO", menuName = "Enemy Behavior/MeleePursuit_SO")]
public class MeleePursuit_SO : EngageBehavior_SO
{

    public override void Perform(GameObject enemy_obj, ScanForCharacters _scanForCharacters, NavMeshAgent _navMeshAgent, EntityStats _entityStats)
    {
        if (_scanForCharacters.targeted_character != null)
        {
            _navMeshAgent.speed = _entityStats.running_speed;

            RaycastHit hit;
            Vector3 direction = (_scanForCharacters.targeted_character.transform.position - enemy_obj.transform.position).normalized;
            float meleeDistance = _entityStats.equipped_meleeWeapon.MeleeWeapon.melee_reach + _entityStats.entity_radius;

            if (Physics.Raycast(enemy_obj.transform.position, direction, out hit, meleeDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform == _scanForCharacters.targeted_character.transform)
                {
                    _navMeshAgent.destination = enemy_obj.transform.position;

                    /*
                    if (!isMeleeAttacking)
                    {
                        isMeleeAttacking = true;

                        Skill_SO skill = _entityStats.active_skill;
                        skill.Use(gameObject, obj_to_pursue);

                        StartCoroutine(MeleeAttackCooldown(_entityStats.equipped_meleeWeapon.attackCooldown));
                    }
                    */
                }
            }
            else
            {
                _navMeshAgent.destination = _scanForCharacters.targeted_character.transform.position;
            }

        }
        
    }

}
