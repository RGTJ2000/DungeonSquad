using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;

public class Enemy_Behavior2 : MonoBehaviour
{
    public EnemyBehaviorStats_SO behaviorStats_SO;

    [SerializeField] private DefaultBehavior_SO defaultBehavior_SO;
    [SerializeField] private EngageBehavior_SO engageBehavior_SO;
    [SerializeField] private AlertBehavior_SO alertBehavior_SO;

    //for BehaviorStats_SO
    [SerializeField] private bool vc_required = true;
    [SerializeField] private Enemy_EngageType engageType = Enemy_EngageType.blind;
    [SerializeField] private bool awareTriggerOccupied = false;
    [SerializeField] private bool engageTriggerOccupied = false;

    [SerializeField] private bool engage_state = false;


    private float aware_radius;
    private float aware_cancelRadius;
    private float engage_radius;
    private float engage_cancelRadius;


    private ScanForCharacters _scanForCharacters;
    private NavMeshAgent _navMeshAgent;
    private EntityStats _entityStats;

    //SphereCollider activeCollider;

    private List<GameObject> ch_list = new List<GameObject>();
    //private bool ch_listFound = false;


    private bool isMeleeAttacking = false;
    private bool isRangedAttacking = false;
    private bool isMagicAttacking = false;

    void Start()
    {
       

        _scanForCharacters = GetComponent<ScanForCharacters>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _entityStats = GetComponent<EntityStats>();

        //fill behavior parameters based on behaviorStats
        if (behaviorStats_SO != null)
        {
            aware_radius = behaviorStats_SO.trigger_aware_radius;
            aware_cancelRadius = behaviorStats_SO.trigger_aware_cancelRadius;
            engage_radius = behaviorStats_SO.trigger_engage_radius;
            engage_cancelRadius = behaviorStats_SO.trigger_engage_cancelRadius;

            vc_required = behaviorStats_SO.visualContact_required;
            defaultBehavior_SO = behaviorStats_SO.defaultBehavior_SO;
            engageBehavior_SO = behaviorStats_SO.engageBehavior_SO;
            alertBehavior_SO = behaviorStats_SO.alertBehavior_SO;

            _entityStats.selected_skill = behaviorStats_SO.engageBehavior_SO.skill_SO; //sets the active skill to that specified in the engageBehavior
        }
        else
        {
            Debug.Log("EntityBehaviorStats_SO is MISSING.");
        }

        //Get referenes to all characters
        ch_list = FindAllCharacters();



    }

    // Update is called once per frame
    void Update()
    {
        

        #region Debug lines
        //debug circle to show trigger zones
        DrawCircle(transform.position, aware_radius, Color.green); //where enemy becomes active
        DrawCircle(transform.position, aware_cancelRadius, Color.green);  //where enemy stops becoming active if not engaged

        DrawCircle(transform.position, engage_radius, Color.red); 
        DrawCircle(transform.position, engage_cancelRadius, Color.red);  //where enemy disengages if engaged


        //debug draw targeted ch line
        if (_scanForCharacters.targeted_character != null)
        {
            if (engage_state)
            {
                Debug.DrawLine(transform.position, _scanForCharacters.targeted_character.transform.position, Color.red);  //engaging this character
            }
            else
            {
                Debug.DrawLine(transform.position, _scanForCharacters.targeted_character.transform.position, Color.yellow); //aware of this character
            }
            
        }
        #endregion

        //clear out null (dead) characters
        CleanCharacterList();

        //Check for Characters in zones
        CheckForCharactersInZones();
      

        // Engagement-Disengagement logic
        if (!engage_state)
        {
            //if not engaged, check if any characters are around to engage
            if (engageTriggerOccupied)
            {
                //write a method "SelectCharacterToAggroFromList" - picks a character in the zone, sets targeted character in scan
                if (!vc_required || _scanForCharacters.SetAndReturnNearestCharacter(_entityStats.visible_distance) != null)
                {
                    engage_state = true;
                }
            }
            
            //if enemy aware (target active) but enemy not engaged and character out of aware disengage range then disengage
            GameObject target = _scanForCharacters.GetTargetedCharacter();
            if ( (target != null && Vector3.Distance(transform.position, target.transform.position) > aware_cancelRadius) || ( target !=null && !_scanForCharacters.CheckCharacterIsVisible(target, aware_cancelRadius) && !vc_required)) 
            {
                CancelAwarenessOfTarget();
            }
            

        }
        else //if engage_state is true
        {
            // Disengagement logic
            GameObject target = _scanForCharacters.GetTargetedCharacter();
            if (target == null)
            {
                DisengageTarget();

            }
            else
            {


                switch (engageType)
                {
                    case Enemy_EngageType.blind:
                        if (target != null)
                        {
                            if (Vector3.Distance(transform.position, target.transform.position) > engage_cancelRadius ||
                                !_scanForCharacters.CheckCharacterIsVisible(target, engage_cancelRadius))
                            {
                                DisengageTarget();
                            }
                        }

                        break;
                    case Enemy_EngageType.dumb:
                        if (target != null && !_scanForCharacters.CheckCharacterIsVisible(target, _entityStats.visible_distance))
                        {
                            DisengageTarget();
                        }
                        break;

                    case Enemy_EngageType.lazy:
                        if (target != null && Vector3.Distance(transform.position, target.transform.position) > engage_cancelRadius)
                        {
                            DisengageTarget();
                        }
                        break;

                    case Enemy_EngageType.tenacious:
                        if (target != null &&
                            Vector3.Distance(transform.position, target.transform.position) > engage_cancelRadius &&
                            !_scanForCharacters.CheckCharacterIsVisible(target, _entityStats.visible_distance) )
                        {
                            DisengageTarget();
                        }
                        break;

                    case Enemy_EngageType.unstoppable:
                        // No disengagement
                        break;

                    default:
                        break;
                }
            }
        }

        //Behavior execution
        if (_navMeshAgent.isOnNavMesh)
        {
            if (engage_state)
            {
                engageBehavior_SO.Perform(gameObject, _scanForCharacters.targeted_character);

                //engageBehavior_SO.skill_SO.Use(gameObject, _scanForCharacters.targeted_character);
                /*
                 * This is using engageBehavior set to MeleePursuit. I changed the skillSO to Assault.
                engageBehavior_SO.Perform(gameObject, _scanForCharacters, _navMeshAgent, _entityStats);
                HandleCombat();
                */
            }
            else if (awareTriggerOccupied)
            {
                
                alertBehavior_SO.Perform(gameObject, _scanForCharacters, _navMeshAgent, _entityStats, aware_radius);
            }
            else
            {

                defaultBehavior_SO.Perform(gameObject, _navMeshAgent);
            }


        }
    }

    private void CheckForCharactersInZones()
    {
        if (ch_list.Count > 0)
        {
            awareTriggerOccupied = CheckTriggerZone(aware_radius);
            engageTriggerOccupied = CheckTriggerZone(engage_radius);
        }
    }

    private List<GameObject> FindAllCharacters()
    {
        return GameObject.FindGameObjectsWithTag("Character").ToList();
    }

    private bool CheckTriggerZone(float radius)
    {
        bool triggerTripped = false;
        foreach (GameObject character in ch_list)
        {
            float distance = Vector3.Distance(transform.position, character.transform.position);
            if (distance <= radius)
            {
                triggerTripped = true;
            }
        }
        if (triggerTripped)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void DrawCircle(Vector3 position, float radius, Color color)
    {
        for (int i = 0; i < 30; i++)
        {
            Debug.DrawLine(position + (Quaternion.AngleAxis((360 / 30) * i, Vector3.up) * transform.forward * radius), position + (Quaternion.AngleAxis((360 / 30) * (i + 1), Vector3.up) * transform.forward * radius), color);
        }
    }
    private void DisengageTarget()
    {
       
        engage_state = false;

        if (!_navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.enabled = true;
        }

        _navMeshAgent.ResetPath();

        _scanForCharacters.targeted_character = null;

    }

    private void CancelAwarenessOfTarget()
    {
      

        _scanForCharacters.targeted_character = null;

    }

    private void HandleCombat()
    {
        GameObject target = _scanForCharacters.targeted_character;

        //case switch to combat action as defined by engageBehavior_SO;
        switch (engageBehavior_SO.combatType)
        {
            case (CombatType.melee):
                if (target != null)
                {
                    RaycastHit hit;
                    Vector3 direction = (target.transform.position - transform.position).normalized;
                    float maxDistance = _entityStats.equipped_meleeWeapon.MeleeWeapon.melee_reach + _entityStats.entity_radius;

                    if (Physics.Raycast(transform.position, direction, out hit, maxDistance, ~0, QueryTriggerInteraction.Ignore))
                    {
                        if (hit.transform == target.transform)
                        {
                            if (!isMeleeAttacking)
                            {
                               

                                isMeleeAttacking = true;

                                if (target != null)
                                {
                                    //Debug.Log("Target= "+target.name+"  GameObject="+gameObject.name);
                                    engageBehavior_SO.skill_SO.Use(gameObject, target);


                                }
                                StartCoroutine(MeleeAttackCooldown(_entityStats.equipped_meleeWeapon.MeleeWeapon.cycleTime));
                            }
                        }
                    }

                }

                break;

            case (CombatType.ranged):

                if (target != null)
                {
                    Vector3 target_direction = target.transform.position - transform.position;
                    target_direction.y = 0;

                    if (!isRangedAttacking && Vector3.Angle(transform.forward, target_direction) < 10f)
                    {
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, _entityStats.visible_distance, ~0, QueryTriggerInteraction.Ignore) && hit.transform.gameObject.tag == "Character")
                        {
                            isRangedAttacking = true;
                            //Debug.Log("Calling Ranged Skill. target=" + target.name);
                            engageBehavior_SO.skill_SO.Use(gameObject, target);

                            StartCoroutine(RangedAttackCooldown(_entityStats.equipped_rangedWeapon.RangedWeapon.cycleTime));
                        }
                    }

                }
                break;

            case (CombatType.magic):

                if (target != null)
                {
                    Vector3 target_direction = target.transform.position - transform.position;
                    target_direction.y = 0;

                    if (!isMagicAttacking && Vector3.Angle(transform.forward, target_direction) < 10f)
                    {
                        RaycastHit hit;

                        if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit, _entityStats.visible_distance, ~0, QueryTriggerInteraction.Ignore) && hit.transform.gameObject.tag == "Character")
                        {
                            isMagicAttacking = true;

                            engageBehavior_SO.skill_SO.Use(gameObject, target);

                            Skill_SO skill = engageBehavior_SO.skill_SO;
                            if (skill is Spell_SO spell)
                            {
                                StartCoroutine(WaitforCastingToComplete(spell.castingTime));
                            }
                        }
                    }

                }

                break;


            default:
                break;
        }
    }

    IEnumerator MeleeAttackCooldown(float meleeTime)
    {
        yield return new WaitForSeconds(meleeTime);

        isMeleeAttacking = false; // Reset the cooldown flag
    }
    IEnumerator RangedAttackCooldown(float rangedCooldown)
    {
        yield return new WaitForSeconds(rangedCooldown);
        isRangedAttacking = false;
    }

    IEnumerator WaitforCastingToComplete(float castingTime)
    {
        yield return new WaitForSeconds(castingTime);
        isMagicAttacking = false;
    }

    private void CleanCharacterList()
    {
        // Remove null or destroyed objects from ch_list
        ch_list.RemoveAll(character => character == null);

        // Optional: Log for debugging
        /*
        if (ch_list.Count > 0)
        {
            Debug.Log($"Cleaned ch_list, remaining count: {ch_list.Count}");
        }
        */
    }
}
