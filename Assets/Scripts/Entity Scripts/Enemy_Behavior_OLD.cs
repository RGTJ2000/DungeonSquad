using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static TriggerZone;
using System.Linq;
using System.Collections.Generic;

public class Enemy_Behavior : MonoBehaviour
{
    public EnemyBehaviorStats_SO behaviorStats_SO;


    private bool vc_required = true;

    //for Behavior_SO
    private Entity_DefaultBehavior defaultBehavior = Entity_DefaultBehavior.idle;
    [SerializeField] private Enemy_EngageType engageType = Enemy_EngageType.blind;


    private bool activeTriggerEntered = false;
    private bool farTriggerEntered = false;
    private bool nearTriggerEntered = false;


    [SerializeField] private bool activeTriggerFilled = false;
    [SerializeField] private bool farTriggerFilled = false;
    [SerializeField] private bool nearTriggerFilled = false;

    private float activeRadius;
    private float farRadius;
    private float nearRadius;

    private GameObject farTriggerObject;
    private GameObject nearTriggerObject;

    [SerializeField] private bool engage_state = false;

    private ScanForCharacters _scanForCharacters;
    private NavMeshAgent _navMeshAgent;
    private EntityStats _entityStats;

    private GameObject[] visibleTargets_array;


    private DefaultBehavior_SO defaultBehavior_SO;
    private EngageBehavior_SO engageBehavior_SO;
    private AlertBehavior_SO alertBehavior_SO;

    private bool isMeleeAttacking = false;
    private bool isRangedAttacking = false;
    private bool isMagicAttacking = false;

    SphereCollider activeCollider;

    private List<GameObject> ch_list = new List<GameObject>();
    private bool ch_listFound = false;

    private bool disengagePerformed = false;

    void Start()
    {
        /*
        // Create child objects for triggers
        farTriggerObject = new GameObject("FarTrigger");
        nearTriggerObject = new GameObject("NearTrigger");

        // Parent them to this GameObject
        farTriggerObject.transform.SetParent(transform, false);
        nearTriggerObject.transform.SetParent(transform, false);

        // Add colliders
        SphereCollider farCollider = farTriggerObject.AddComponent<SphereCollider>();
        SphereCollider nearCollider = nearTriggerObject.AddComponent<SphereCollider>();
        */

        //try just adding one activeRadius collider
        activeCollider = gameObject.AddComponent<SphereCollider>();
        activeCollider.radius = behaviorStats_SO.trigger_activeRadius;
        activeCollider.isTrigger = true;



        if (behaviorStats_SO != null)
        {
            activeRadius = behaviorStats_SO.trigger_activeRadius;
            farRadius = behaviorStats_SO.trigger_farRadius;
            nearRadius = behaviorStats_SO.trigger_nearRadius;

            vc_required = behaviorStats_SO.visualContact_required;
            defaultBehavior = behaviorStats_SO.defaultBehavior; //is this redundant?
            defaultBehavior_SO = behaviorStats_SO.defaultBehavior_SO;
            engageBehavior_SO = behaviorStats_SO.engageBehavior_SO;
            alertBehavior_SO = behaviorStats_SO.alertBehavior_SO;

            /*
            farCollider.isTrigger = true;
            farCollider.radius = behaviorStats_SO.trigger_farRadius;
            nearCollider.isTrigger = true;
            nearCollider.radius = behaviorStats_SO.trigger_nearRadius;
           

            farTriggerObject.AddComponent<TriggerZone>().Setup(this, TriggerZone.ZoneType.Far);
            nearTriggerObject.AddComponent<TriggerZone>().Setup(this, TriggerZone.ZoneType.Near);
            */



        }
        else
        {
            Debug.Log("EntityBehaviorStats_SO is MISSING.");
        }

        _scanForCharacters = GetComponent<ScanForCharacters>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _entityStats = GetComponent<EntityStats>();
    }



    // Update is called once per frame
    void Update()
    {
        Debug.Log($"Start of Update: target = {(_scanForCharacters.targeted_character != null ? _scanForCharacters.targeted_character.name : "null")}, engage_state = {engage_state}, farTriggerFilled = {farTriggerFilled}");

        DrawCircle(transform.position, behaviorStats_SO.trigger_activeRadius, Color.green);
        DrawCircle(transform.position, behaviorStats_SO.trigger_farRadius, Color.red);
        DrawCircle(transform.position, behaviorStats_SO.trigger_nearRadius, Color.yellow);

        if (disengagePerformed)
        {
            Debug.Log("Update: Targeted character = " + _scanForCharacters.targeted_character.name);
            disengagePerformed = false;
        }
        if (_scanForCharacters.targeted_character != null)
        {
            Debug.DrawLine(transform.position, _scanForCharacters.targeted_character.transform.position, Color.red);
        }

        if (activeTriggerFilled)
        {
            //scan for characters and check distances
            if (!ch_listFound)
            {
                ch_list = FindAllCharacters();
                ch_listFound = true;
            }

            if (ch_list.Count > 0)
            {
                activeTriggerFilled = CheckTriggerZone(activeRadius + _entityStats.entity_radius);
                farTriggerFilled = CheckTriggerZone(farRadius);
                nearTriggerFilled = CheckTriggerZone(nearRadius);

            }
            else
            {
                activeTriggerFilled = false; // Reset if no characters remain
                farTriggerFilled = false;
                nearTriggerFilled = false;
            }


        }

        // Engagement logic
        if (!engage_state)
        {
            // Only attempt to engage if not already engaged
            if (farTriggerFilled)
            {
                if (!vc_required || _scanForCharacters.SetAndReturnNearestCharacter(_entityStats.visible_distance) != null)
                {
                    engage_state = true;
                }
            }
        }
        else
        {
            // Disengagement logic
            GameObject target = _scanForCharacters.GetTargetedCharacter();
            switch (engageType)
            {
                case Enemy_EngageType.blind:
                    if (target != null)
                    {
                        if (Vector3.Distance(transform.position, target.transform.position) > behaviorStats_SO.trigger_activeRadius ||
                            !_scanForCharacters.CheckCharacterIsVisible(target, behaviorStats_SO.trigger_activeRadius))
                        {
                            DisengageParty();
                            // Do NOT call SetAndReturnNearestCharacter here unless re-engaging
                        }
                    }
                    break;

                case Enemy_EngageType.dumb:
                    if (target != null && !_scanForCharacters.CheckCharacterIsVisible(target, _entityStats.visible_distance))
                    {
                        DisengageParty();
                    }
                    break;

                case Enemy_EngageType.lazy:
                    if (target != null && Vector3.Distance(transform.position, target.transform.position) > behaviorStats_SO.trigger_activeRadius)
                    {
                        DisengageParty();
                    }
                    break;

                case Enemy_EngageType.tenacious:
                    if (target != null &&
                        Vector3.Distance(transform.position, target.transform.position) > behaviorStats_SO.trigger_activeRadius &&
                        !_scanForCharacters.CheckCharacterIsVisible(target, _entityStats.visible_distance) &&
                        !activeTriggerFilled)
                    {
                        DisengageParty();
                    }
                    break;

                case Enemy_EngageType.unstoppable:
                    // No disengagement
                    break;
            }
        }


        // Behavior execution
        if (_navMeshAgent.isOnNavMesh)
        {
            if (engage_state)
            {
                engageBehavior_SO.Perform(gameObject, _scanForCharacters, _navMeshAgent, _entityStats);
                HandleCombat();
            }
            else if (activeTriggerFilled)
            {
                alertBehavior_SO.Perform(gameObject, _scanForCharacters, _navMeshAgent, _entityStats, activeRadius);
            }
            else
            {
                defaultBehavior_SO.Perform(gameObject, _navMeshAgent);
            }
        }
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
                float maxDistance = _entityStats.equipped_meleeWeapon.melee_reach + _entityStats.entity_radius;

                if (Physics.Raycast(transform.position, direction, out hit, maxDistance, ~0, QueryTriggerInteraction.Ignore))
                {
                    if (hit.transform == target.transform)
                    {
                        if (!isMeleeAttacking)
                        {
                            isMeleeAttacking = true;

                            engageBehavior_SO.skill_SO.Use(gameObject, target);

                            StartCoroutine(MeleeAttackCooldown(_entityStats.equipped_meleeWeapon.attackCooldown));
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
                        Debug.Log("Calling Ranged Skill. target=" + target.name);
                        engageBehavior_SO.skill_SO.Use(gameObject, target);

                        StartCoroutine(RangedAttackCooldown(_entityStats.equipped_rangedWeapon.attackCooldown));
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






void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character") && !activeTriggerFilled)
        {
            activeTriggerFilled = true;
        }
    }


    public void SetFarTrigger(bool entered) => farTriggerEntered = entered;
    public void SetNearTrigger(bool entered) => nearTriggerEntered = entered;


    public void SetFarTriggerFill(bool fill) => farTriggerFilled = fill;
    public void SetNearTriggerFill(bool fill) => nearTriggerFilled = fill;

    void DrawCircle(Vector3 position, float radius, Color color)
    {

        for (int i = 0; i < 30; i++)
        {
            Debug.DrawLine(position + (Quaternion.AngleAxis((360 / 30) * i, Vector3.up) * transform.forward * radius), position + (Quaternion.AngleAxis((360 / 30) * (i + 1), Vector3.up) * transform.forward * radius), color);
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

    private void DisengageParty()
    {
        engage_state = false;
        _scanForCharacters.targeted_character = null;
        Debug.Log($"DisengageParty: targeted_character = {_scanForCharacters.targeted_character?.name ?? "null"}, farTriggerFilled = {farTriggerFilled}");
        
        disengagePerformed = true;
    }
}
