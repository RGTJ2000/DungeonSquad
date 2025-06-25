using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "TripleStrike_SO", menuName = "Melee Skills/TripleStrike_SO")]
public class TripleStrike_SO : Skill_SO
{
    [SerializeField] Skill_SO simpleAssault_SO;
    [SerializeField] GameObject tripleStrike_prefab;

    private EntityStats _attackerStats;
    private NavMeshAgent _navMeshAgent;
    private SkillCooldownTracker _cooldownTracker;
    public override void Use(GameObject attacker, GameObject target)
    {

        _attackerStats = attacker.GetComponent<EntityStats>();
        _navMeshAgent = attacker.GetComponent<NavMeshAgent>();
        _cooldownTracker = attacker.GetComponent<SkillCooldownTracker>();


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

                if (_cooldownTracker.GetRemainingCooldown(this) == 0)
                {
                    //instantiate triple hit prefab

                    //Debug.Log("Instantiating Triple Strike.");
                    GameObject tripleStrike_obj = Instantiate(tripleStrike_prefab, attacker.transform);

                    tripleStrike_obj.GetComponent<TripleStrikeGuidance>().SetTripleStrikeParameters(attacker, target);

                    _cooldownTracker.StartCooldown(this, cooldown); //start cooldown for triple strike instantiation

                    //after instantiaion of TripleStrike set the skill performing to simple melee assault
                    /*
                     *  UNIFY CH_Behavior and Enemy_Behavior to accept the same skills???
                     * 
                     */
                    if (attacker.TryGetComponent<Ch_Behavior>(out Ch_Behavior _chBehavior))
                    {
                        //if a character is using this, then set the skill to simple assault now
                        _chBehavior.SetSkillPerforming(simpleAssault_SO);
                        //and set the simple assault cooldown so that it proc's after triple strike
                        _cooldownTracker.StartCooldown(simpleAssault_SO, simpleAssault_SO.cooldown);
                    }
                }
                /*
                else
                {
                   

                    
                    //perform single hits if triple strike on cooldown
                    if (_cooldownTracker.GetRemainingCooldown(meleeAttack_SO) == 0)
                    {
                        if (meleeAttack_SO != null)
                        {
                            meleeAttack_SO.Use(attacker, target);
                            //start cooldown is called by th meleeAttack_SO itself
                        }
                    }
                    

                }*/


            } 
            else
            {
                if (_navMeshAgent.isOnNavMesh)
                {
                    _navMeshAgent.destination = target.transform.position;

                }
            }
        }



    }
}
    

