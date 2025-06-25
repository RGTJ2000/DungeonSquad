using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New Assault SO", menuName = "Melee Skills/Assault_SO")]
public class Assault_SO : Skill_SO
{
    [SerializeField] Skill_SO meleeAttack_SO;

    private EntityStats _attackerStats;
    private NavMeshAgent _navMeshAgent;
    private SkillCooldownTracker _cooldownTracker;

    public override void Use(GameObject attacker, GameObject target)
    {

        _attackerStats = attacker.GetComponent<EntityStats>();
        _navMeshAgent = attacker.GetComponent<NavMeshAgent>();
        _cooldownTracker = attacker.GetComponent<SkillCooldownTracker>();

        //MOVE ATTACKER TO CLOSE DISTANCE WITH TARGET
        RaycastHit hit;
        Vector3 direction = (target.transform.position - attacker.transform.position).normalized;
        float maxDistance = _attackerStats.equipped_meleeWeapon.MeleeWeapon.melee_reach + _attackerStats.entity_radius;

        if (!Physics.Raycast(attacker.transform.position, direction, out hit, maxDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            if (_navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.destination = target.transform.position;

            }
        }
        else
        {
            if (hit.transform == target.transform)
            {
                if (_navMeshAgent.isOnNavMesh)
                {
                    _navMeshAgent.destination = attacker.transform.position;

                }

                if (_cooldownTracker != null && _cooldownTracker.GetRemainingCooldown(meleeAttack_SO) == 0)
                {
                    //PERFORM ACTUAL MELEE ATTACK
                    if (meleeAttack_SO != null)
                    {
                        meleeAttack_SO.Use(attacker, target);

                    }
                }


            } else
            {
                if (_navMeshAgent.isOnNavMesh)
                {
                    _navMeshAgent.destination = target.transform.position;

                }
            }
        }



    }
}
