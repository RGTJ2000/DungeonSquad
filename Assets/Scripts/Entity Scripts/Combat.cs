using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class Combat : MonoBehaviour
{
    private bool isEngaging = false;
    private bool isMeleeAttacking = false;
    private bool isRangedAttacking = false;
    private bool isMagicAttacking = false;
    private bool magic_completed = false;
    public bool isIncanting = false;

    private string skill_performing;

    private GameObject target_obj = null;

    private EntityStats _entityStats;
    private MissileLauncher _missileLauncher;
    private MagicHandler _magicHandler;
    private IncantHandler _incantHandler;
    private NavMeshAgent _navMeshAgent;
    private Ch_Behavior _chMove = null;
    private EnemyMove _enemyMove = null;
    private Health _health;
    private FloatTextDisplay _floatTextDisplay;
    private StatusTracker _statusTracker;


    void Start()
    {
        _entityStats = GetComponent<EntityStats>();
        _health = GetComponent<Health>();
        _missileLauncher = GetComponent<MissileLauncher>();
        _magicHandler = GetComponent<MagicHandler>();
        _incantHandler = GetComponent<IncantHandler>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _floatTextDisplay = GetComponent<FloatTextDisplay>();
        _statusTracker = GetComponent<StatusTracker>();

        if (TryGetComponent<Ch_Behavior>(out _chMove))
        {
            TryGetComponent<EnemyMove>(out _enemyMove);
        }
    }

    void Update()
    {
        if (isEngaging)
        {
            if (target_obj != null)
            {
                if (skill_performing == "melee")
                {
                    MeleeAttackTarget(target_obj);
                }
                else if (skill_performing == "ranged")
                {
                    RangedAttackTarget(target_obj);
                }
                /*else if (skill_performing == "magic")
                {
                    MagicAttackTarget(target_obj);
                }*/
                else if (skill_performing == "incant")
                {
                    IncantTarget(target_obj);
                }

            }
        }
    }

    private void MeleeAttackTarget(GameObject target_object)
    {
        RaycastHit hit;
        Vector3 direction = (target_obj.transform.position - transform.position).normalized;
        float maxDistance = _entityStats.equipped_meleeWeapon.MeleeWeapon.melee_reach + _entityStats.entity_radius;

        if (Physics.Raycast(transform.position, direction, out hit, maxDistance))
        {
            if (hit.transform == target_obj.transform)
            {
                _navMeshAgent.destination = transform.position;
                if (!isMeleeAttacking)
                {
                    CombatManager.Instance.ResolveMelee(gameObject, target_object);
                    StartCoroutine(MeleeAttackCooldown());
                }
            }
        }
        else
        {
            _navMeshAgent.destination = target_obj.transform.position;
        }
    }
    private void RangedAttackTarget(GameObject target_object)
    {

        Vector3 target_direction = target_object.transform.position - transform.position;
        target_direction.y = 0; // Optional: Keep character upright (no tilt on Y-axis)

        if (target_direction != Vector3.zero) // Avoid errors if direction is zero
        {
            // Target rotation to look at the enemy
            Quaternion targetRotation = Quaternion.LookRotation(target_direction);

            // Smoothly interpolate from current rotation to target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }

        //transform.LookAt(target_object.transform.position);  //face target

        //check if target is visible, if visible and not in rangedCooldown then launch missile
        if (!isRangedAttacking && Vector3.Angle(transform.forward, target_direction) < 10f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, target_object.transform.position - transform.position, out hit, _entityStats.visible_distance) && hit.transform.gameObject.tag == "Enemy")
            {
                _missileLauncher.LaunchMissile(target_obj, gameObject);
                StartCoroutine(RangedAttackCooldown());
            }
        }
    }

    /*
    
    private void MagicAttackTarget(GameObject target_object)
    {
        Vector3 target_direction = target_object.transform.position - transform.position;
        target_direction.y = 0;

        if (target_direction != Vector3.zero) // Avoid errors if direction is zero
        {
            Quaternion targetRotation = Quaternion.LookRotation(target_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }

        if (!isMagicAttacking && Vector3.Angle(transform.forward, target_direction) < 10f) //if not currently casting and last spell completed and facing target do this
        {

            isMagicAttacking = true;
            magic_completed = false;

            Skill_SO skill = _entityStats.selected_skill;
            if (skill is CastSpell_SO spell)
            {
                Debug.Log("Magic attack beginnging for " + spell.skill_name);
                spell.Use(gameObject, target_obj);
                StartCoroutine(WaitforCastingToComplete(spell.castingTime));
            }
            


        }
        else if (magic_completed) //when magicHandler is casting, only do this 
        {

            isMagicAttacking = false;
            magic_completed = false; //reset the magic_completed variable
            //turn off assault and return to formation
            if (_chMove != null)
            {
                _chMove.CancelEngage();
            }
            else if (_enemyMove != null)
            {
                //return to other behavior
            }

        }
    }
      
    */
    private void IncantTarget(GameObject target_object)
    {

        Vector3 target_direction = target_object.transform.position - transform.position;
        target_direction.y = 0; // Optional: Keep character upright (no tilt on Y-axis)

        if (target_direction != Vector3.zero) // Avoid errors if direction is zero
        {
            // Target rotation to look at the enemy
            Quaternion targetRotation = Quaternion.LookRotation(target_direction);

            // Smoothly interpolate from current rotation to target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }

        if (!isIncanting && Vector3.Angle(transform.forward, target_direction) < 10f)
        {
            //begin incant
            _incantHandler.CastActiveIncant(target_object);
            //incant spells will continue while incant_active, when incantobject heard a focus change they destroy themselves.
            isIncanting = true;
        }

    }

    public void SetEngageStatus(bool engageStatus)
    {
        isEngaging = engageStatus;
    }
    public void SetSkillPerforming(string skill)
    {
        skill_performing = skill;
    }
    public void SetTargetObject(GameObject obj)
    {
        target_obj = obj;
    }
    public void SetIncantStatus(bool incantStatus)
    {
        isIncanting = incantStatus;
    }

    public bool ActiveTargetDifferentFrom(GameObject newTarget)
    {
        if (target_obj != newTarget)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //public void ReceiveCombatResult(bool isHit, string damageType, float damageReceived, GameObject attacker)
    public void ReceiveCombatResult(CombatResult combat_result)
    {
       

        if (combat_result.resultType == CombatResultType.hit || combat_result.resultType == CombatResultType.critical)
        {
            foreach (DamageResult damageResult in combat_result.damageResultList)
            {
                //show the damage as floating text
                _floatTextDisplay.ShowFloatDamage(damageResult.DamageAmount, combat_result.resultType, damageResult.DamageType);

                //allot the damage to either health or status
                switch (damageResult.DamageType)
                {
                    case DamageType.physical:
                        // Handle physical damage
                        _health.TakeDamage(damageResult.DamageAmount);
                        break;

                    case DamageType.confusion:
                        // Handle confusion effect
                        _statusTracker.ReceiveStatusCount(damageResult.DamageAmount, damageResult.DamageType);
                        //add points to statusTracker
                        break;

                    case DamageType.fear:
                        // Handle fear effect
                        //add points to statusTracker
                        _statusTracker.ReceiveStatusCount(damageResult.DamageAmount, damageResult.DamageType);
                        break;

                    case DamageType.fire:
                        // Handle fire damage (maybe apply burning)
                        //add points to statusTracker
                        _health.TakeDamage(damageResult.DamageAmount);
                        _statusTracker.ReceiveStatusCount(damageResult.DamageAmount, damageResult.DamageType);
                        break;

                    case DamageType.frost:
                        // Handle frost damage (maybe slow target)
                        //add points to statusTracker
                        _statusTracker.ReceiveStatusCount(damageResult.DamageAmount, damageResult.DamageType);
                        break;

                    case DamageType.poison:
                        // Handle poison damage (maybe apply DoT)
                        //add points to statusTracker
                        _statusTracker.ReceiveStatusCount(damageResult.DamageAmount, damageResult.DamageType);
                        break;

                    case DamageType.sleep:
                        // Handle sleep effect (maybe disable actions)
                        //add points to statusTracker
                        _statusTracker.ReceiveStatusCount(damageResult.DamageAmount, damageResult.DamageType);
                        break;

                    default:
                        // Optional: catch unexpected or unhandled types
                        Debug.LogWarning("Unhandled damage type: " + damageResult.DamageType);
                        break;

                }


            }
            
        }
        else
        {
            //_health.Miss();
            _floatTextDisplay.ShowFloatMiss(combat_result.resultType);
        }

        /*
        if (GetComponent<EnemyMove>() != null)
        {
            GetComponent<EnemyMove>().CheckAndSetAttacker(combat_result.attacker);
        }
        */


    }

    IEnumerator MeleeAttackCooldown()
    {
        isMeleeAttacking = true; // Set the cooldown flag

        float waitTime = _entityStats.equipped_meleeWeapon.MeleeWeapon.cycleTime / StatScale(_entityStats.dex_adjusted);

        // Wait for the cooldown duration
        yield return new WaitForSeconds(waitTime);

        isMeleeAttacking = false; // Reset the cooldown flag
    }

    IEnumerator RangedAttackCooldown()
    {
        isRangedAttacking = true;


        float waitTime = _entityStats.equipped_rangedWeapon.RangedWeapon.cycleTime / StatScale(_entityStats.dex_adjusted);
        Debug.Log("range wait time = " + waitTime);
        // Wait for the cooldown duration
        yield return new WaitForSeconds(waitTime);

        
        isRangedAttacking = false;
    }

    IEnumerator WaitforCastingToComplete(float castingTime)
    {
        float waitTime = castingTime / StatScale(_entityStats.int_adjusted);
        yield return new WaitForSeconds(waitTime);
        magic_completed = true;
    }

    private float StatScale(float stat)
    {
        return (stat - 50f) / 50f;
    }

}
