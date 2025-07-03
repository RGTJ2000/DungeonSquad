using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;
using static UnityEngine.GraphicsBuffer;

public class Enemy_Behavior2 : MonoBehaviour
{
    public EnemyBehaviorStats_SO behaviorStats_SO;

    [SerializeField] private DefaultBehavior_SO defaultBehavior_SO;
    [SerializeField] private EngageBehavior_SO engageBehavior_SO;
    [SerializeField] private AlertBehavior_SO alertBehavior_SO;

    [SerializeField] private TargetSelection_SO targetSelection_SO;

    //for BehaviorStats_SO
    [SerializeField] private bool vc_required = true;
    [SerializeField] private Enemy_EngageType engageType = Enemy_EngageType.blind;
    [SerializeField] private bool awareTriggerOccupied = false;
    [SerializeField] private bool engageTriggerOccupied = false;

    [SerializeField] private bool engage_state = false;
    [SerializeField] private float engageDamageThreshold = 10f;


    private float aware_radius;
    private float aware_cancelRadius;
    private float engage_radius;
    private float engage_cancelRadius;


    private ScanForCharacters _scanForCharacters;
    private NavMeshAgent _navMeshAgent;
    private EntityStats _entityStats;
    private ThreatTracker _threatTracker;


    private List<GameObject> ch_list = new List<GameObject>();

    private List<GameObject> targetList = new List<GameObject>();

    private GameObject targetedCharacter = null;

    void Start()
    {


        _scanForCharacters = GetComponent<ScanForCharacters>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _entityStats = GetComponent<EntityStats>();
        _threatTracker = GetComponent<ThreatTracker>();

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
            targetSelection_SO = behaviorStats_SO.targetSelection_SO;

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


        //When aware, check threat levels, if top threat level is zero then pick based on evaluation_SO. If not zero then, select that top threat as target.
        //On receiving hit from attacker, reevaluate threat level comparing currently engaged and new attacker. If higher from new attacker, then keep on engaged target.

        #region Debug lines
        //debug circle to show trigger zones
        DrawCircle(transform.position, aware_radius, Color.green); //where enemy becomes active
        DrawCircle(transform.position, aware_cancelRadius, Color.green);  //where enemy stops becoming active if not engaged

        DrawCircle(transform.position, engage_radius, Color.red);
        DrawCircle(transform.position, engage_cancelRadius, Color.red);  //where enemy disengages if engaged


        //debug draw targeted ch line
        if (targetedCharacter != null)
        {
            if (engage_state)
            {
                Debug.DrawLine(transform.position, targetedCharacter.transform.position, Color.red);  //engaging this character
            }
            else
            {
                Debug.DrawLine(transform.position, targetedCharacter.transform.position, Color.yellow); //aware of this character
            }

        }
       


        #endregion

        //clear out null (dead) characters
        CleanCharacterList();

        //Check for Characters in zones
        CheckForCharactersInZones();


        // Engagement-Disengagement logic
        if (awareTriggerOccupied)
        {
            if (targetedCharacter == null)
            {
                targetList.Clear();

                if (!vc_required)
                {
                    targetList = ch_list;
                }
                else
                {
                    targetList = _scanForCharacters.ScanVisibleCharacters(_entityStats.visible_distance).ToList();
                }

                targetedCharacter = targetSelection_SO.Perform(gameObject, targetList);

                _scanForCharacters.targeted_character = targetedCharacter;

            }





            //if enemy aware (target active) but enemy not engaged and character out of aware disengage range then disengage
            //GameObject target = _scanForCharacters.GetTargetedCharacter();




        }

        if (!engage_state)
        {
            if (engageTriggerOccupied && targetedCharacter != null)
            {
                engage_state = true;
            }

            if ((!vc_required && targetedCharacter != null && Vector3.Distance(transform.position, targetedCharacter.transform.position) > aware_cancelRadius) || (vc_required && targetedCharacter != null && !_scanForCharacters.CheckCharacterIsVisible(targetedCharacter, aware_cancelRadius)))
            {
                CancelAwarenessOfTarget();
            }


        }
        else //if engage_state is true
        {
            // Disengagement logic
            //GameObject target = _scanForCharacters.GetTargetedCharacter();
            if (targetedCharacter == null)
            {
                DisengageTarget();

            }
            else
            {

                switch (engageType)
                {
                    //add a threat check on distance disengages
                    //also add engage when threat level > 0

                    case Enemy_EngageType.blind:

                        if (Vector3.Distance(transform.position, targetedCharacter.transform.position) > engage_cancelRadius ||
                            !_scanForCharacters.CheckCharacterIsVisible(targetedCharacter, engage_cancelRadius))
                        {
                            DisengageTarget();
                        }
                        break;
                    case Enemy_EngageType.dumb:
                        if (!_scanForCharacters.CheckCharacterIsVisible(targetedCharacter, _entityStats.visible_distance))
                        {
                            DisengageTarget();
                        }
                        break;

                    case Enemy_EngageType.lazy:
                        if (Vector3.Distance(transform.position, targetedCharacter.transform.position) > engage_cancelRadius)
                        {
                            DisengageTarget();
                        }
                        break;

                    case Enemy_EngageType.tenacious:
                        if (Vector3.Distance(transform.position, targetedCharacter.transform.position) > engage_cancelRadius &&
                            !_scanForCharacters.CheckCharacterIsVisible(targetedCharacter, _entityStats.visible_distance))
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


                if (targetedCharacter != null)
                {

                    //check if top threat is greater than currently targeted character
                    //if above theshhold then switch targeted character
                }
            }
        }

        //Behavior execution
        if (_navMeshAgent.isOnNavMesh)
        {
            if (engage_state)
            {
                if (targetedCharacter != null)
                {
                    engageBehavior_SO.Perform(gameObject, targetedCharacter);

                }



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
        targetedCharacter = null;

    }




    private void CleanCharacterList()
    {
        // Remove null or destroyed objects from ch_list
        ch_list.RemoveAll(character => character == null);


    }


    public void OnDamageCheckTargetedCharacter(GameObject attacker)
    {
        if (!engage_state)
        {
            engage_state = true;
            targetedCharacter = attacker;
            return;
        }
        else //currently engaging
        {
            if (targetedCharacter == null)
            {
                targetedCharacter = attacker;
            }
            else
            {
                float currentThreat = _threatTracker.GetThreatLevel(targetedCharacter);
                float newThreat = _threatTracker.GetThreatLevel(attacker);

                if (newThreat >= currentThreat + engageDamageThreshold)
                {
                    targetedCharacter = attacker;
                }

            }

        }





    }
}
