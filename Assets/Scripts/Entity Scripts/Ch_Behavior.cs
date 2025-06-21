using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System;
using UnityEditor.Experimental.GraphView;
using static UnityEngine.UI.CanvasScaler;
using Unity.VisualScripting;
using static UnityEngine.GraphicsBuffer;

public class Ch_Behavior : MonoBehaviour
{
    public event Action OnIncantFocusChanged;

    public int slot_num;        //set by squad manager at instantiate      
    public GameObject core_obj;

    private CharacterController _controller;
    private SlotProjector _slotprojector;
    private Combat _combat;
    private EntityStats _entityStats;
    private TargetingScan _targetingscan;
    private NavMeshAgent _navMeshAgent;
    private SkillCooldownTracker _skillCooldownTracker;

    private Vector3 core_homing_vector = Vector3.zero;

    public bool isInFormation = true;
    public bool isEngaging = false;
    public bool isPickingUp = false;
    public bool isPerusing = false;
    private bool isReturning = false;
    public Skill_SO skill_performing;

    private bool isMeleeAttacking = false;
    private bool isRangedAttacking = false;
    private bool isMagicAttacking = false;
    private bool magicCompleted = false;

    private bool isIncanting = false;

    private bool waitingForPickup = false;

    private GameObject currentIncantTarget = null;

    public ActionMode actionMode = ActionMode.combat;

    private void OnEnable()
    {
        ItemEvents.OnItemPickedUp += UpdateWaitingForPickup;
    }

    private void OnDisable()
    {
        ItemEvents.OnItemPickedUp -= UpdateWaitingForPickup;
    }

    void Start()
    {
        _slotprojector = core_obj.GetComponent<SlotProjector>();
        _combat = GetComponent<Combat>();
        _controller = GetComponent<CharacterController>();
        _targetingscan = GetComponent<TargetingScan>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _entityStats = GetComponent<EntityStats>();
        _skillCooldownTracker = GetComponent<SkillCooldownTracker>();
    }

    
    

    void FixedUpdate()
    {
        if (isEngaging)
        {
           
            if (_targetingscan.targeted_entity != null)
            {
                _targetingscan.ActivateTargetArrow();

                if (skill_performing.skill_type == "melee")
                {
                    ActiveMelee(_targetingscan.targeted_entity);

                }
                else if (skill_performing.skill_type == "ranged")
                {
                    ActiveRanged(_targetingscan.targeted_entity);
                }
                else if (skill_performing.skill_type == "magic")
                {
                    //magic
                    ActiveMagic(_targetingscan.targeted_entity);
                }
                else if (skill_performing.skill_type == "incant")
                {
                    // healing or incantations

                    ActiveIncant(_targetingscan.targeted_entity);
                }
            }
            else
            {
                if (skill_performing.skill_type != "melee")
                {
                    CancelEngage();
                }
                else if (_targetingscan.SetandReturnNearestTargetEntity("Enemy") == null)
                {
                    CancelEngage();
                }
            }
        }
        else if (isPickingUp)
        {
           
                Debug.Log("PICKUP ITEM NOW");
            //pick up item
            ActivePickup(_targetingscan.targeted_entity);

        }
        else if (isPerusing)
        {
            ActivePerusing();
        }

        if (isInFormation)
        {
            FollowFormation();
        }
    }

    public void ActiveMelee(GameObject obj_to_pursue)
    {
        //change movement behavior
        isInFormation = false;
        _navMeshAgent.enabled = true;
        _controller.enabled = false;
        //set target arrow on
        _targetingscan.target_arrow_on = true;

        skill_performing.Use(gameObject, obj_to_pursue);

    }
    public void ActiveRanged(GameObject obj_to_ranged)
    {
        isInFormation = true;
        _targetingscan.target_arrow_on = true;
        FaceTarget(obj_to_ranged);

        Vector3 target_direction = obj_to_ranged.transform.position - transform.position;
        target_direction.y = 0;

        if (!isRangedAttacking && Vector3.Angle(transform.forward, target_direction) < 10f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, obj_to_ranged.transform.position - transform.position, out hit, _entityStats.visible_distance, ~0, QueryTriggerInteraction.Ignore) && hit.transform.gameObject.tag == "Enemy")
            {
                isRangedAttacking = true;

                //Skill_SO skill = _entityStats.selected_skill;
                Skill_SO skill = skill_performing;
                skill.Use(gameObject, obj_to_ranged);

                StartCoroutine(RangedAttackCooldown(_entityStats.equipped_rangedWeapon.RangedWeapon.cycleTime));
            }
        }

        /*
        _combat.SetEngageStatus(true);
        _combat.SetSkillPerforming(skill_performing.skill_type);

        _combat.SetTargetObject(obj_to_ranged);
        */
    }
    public void ActiveMagic(GameObject obj_to_magic)
    {

        isInFormation = false;
        _controller.enabled = false;
        _navMeshAgent.enabled = true;   //switch to navmeshagentcontrol
        _navMeshAgent.destination = transform.position; //must stand in place for magic

        _targetingscan.target_arrow_on = true;

        FaceTarget(obj_to_magic);

        Vector3 target_direction = obj_to_magic.transform.position - transform.position;
        target_direction.y = 0;

        if (!isMagicAttacking && Vector3.Angle(transform.forward, target_direction) < 10f) //if not currently casting and last spell completed and facing target do this
        {
            Skill_SO skill = _entityStats.selected_skill;
            if (skill is Spell_SO spell)
            {
                //Debug.Log("Magic attack beginnging for " + spell.skill_name);
                spell.Use(gameObject, obj_to_magic);
                StartCoroutine(WaitforCastingToComplete(spell.castingTime));

                if (_skillCooldownTracker != null)
                {
                    _skillCooldownTracker.StartCooldown(skill);
                }

            }

            isMagicAttacking = true;
            magicCompleted = false;


        }
        else if (magicCompleted) //when WaitforCasting is completed
        {

            isMagicAttacking = false;
            magicCompleted = false;
            //turn off assault and return to formation
            CancelEngage();


        }




    }
    public void ActiveIncant(GameObject obj_to_incant)
    {
        isInFormation = false;
        _controller.enabled = false;
        _navMeshAgent.enabled = true;   //switch to navmeshagentcontrol
        _navMeshAgent.destination = transform.position; //must stand in place for incant
        _targetingscan.target_arrow_on = true;

        FaceTarget(obj_to_incant);

        if (obj_to_incant != currentIncantTarget)
        {
            OnIncantFocusChanged?.Invoke(); //alert incant objects that focus has changed
            currentIncantTarget = obj_to_incant;

        }

        Vector3 target_direction = obj_to_incant.transform.position - transform.position;
        target_direction.y = 0; // Optional: Keep character upright (no tilt on Y-axis)

        if (!isIncanting && Vector3.Angle(transform.forward, target_direction) < 10f)
        {
            //MAKE HEAL_SO a subclass of Incant_SO
            Skill_SO skill = _entityStats.selected_skill;
            skill.Use(gameObject, obj_to_incant);

            //_incantHandler.CastActiveIncant(target_object);
            //incant spells will continue while incant_active, when incantobject heard a focus change they destroy themselves.
            isIncanting = true;
        }

        //perform the incant here




        /* _combat.SetEngageStatus(true);
        _combat.SetSkillPerforming(skill_performing.skill_type);
        
        if (_combat.ActiveTargetDifferentFrom(obj_to_incant))
        {
            _combat.SetIncantStatus(false); //stop previous incant
            OnIncantFocusChanged?.Invoke(); //alert incant objects that focus has changed

            Debug.Log("Setting incant Status:" + _combat.isIncanting);
            _combat.SetTargetObject(obj_to_incant);  //sets new active combat target
        }
        */

    }

    private void ActivePickup(GameObject obj_to_pickup)
    {
        //change movement behavior
        isInFormation = false;
        _navMeshAgent.enabled = true;
        _controller.enabled = false;
        //set target arrow on
        _targetingscan.target_arrow_on = true;


        float distance = Vector3.Distance(obj_to_pickup.transform.position, transform.position);
        if (distance > (_entityStats.entity_radius + _entityStats.entity_reach))
        {
            if (_navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.destination = obj_to_pickup.transform.position;

            }

        }
        else
        {
            if (_navMeshAgent.isOnNavMesh)
            {
                _navMeshAgent.destination = transform.position;

            }
            DroppedItemBehavior _dib = obj_to_pickup.GetComponent<DroppedItemBehavior>();
            _dib.ActivatePickup(gameObject);
            waitingForPickup = true;
            isPickingUp = false;
            isPerusing = true;
            Debug.Log("Switching to Perusing. Action mode =" + actionMode);
        }

    }

    private void ActivePerusing()
    {
        //Debug.Log("PERUSING. Action mode = "+actionMode);
        isInFormation = false;
        _navMeshAgent.enabled = true;
        _controller.enabled = false;
        _targetingscan.ActivateTargetingScan();

        if (actionMode == ActionMode.combat && !waitingForPickup)
        {
            Debug.Log("Canceling actions. Action mode="+actionMode);
            CancelActions(); //if no items visible, then return to formation
        }
        else if (_navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.destination = transform.position;
        }
    }

    private void UpdateWaitingForPickup()
    {
        if ( waitingForPickup )
        {
            waitingForPickup = false;
        }
    }

    public bool ActivateAction()
    {

        if (actionMode == ActionMode.combat)
        {
            Skill_SO active_skill = _entityStats.selected_skill;

            if (_skillCooldownTracker == null || !_skillCooldownTracker.IsSkillOnCooldown(active_skill))
            {
                _targetingscan.SetTargetedEntity();  //set targeted entity
                SetSkillPerforming(_entityStats.selected_skill);  //set the skill to perform

                isEngaging = true;  //turn on engage mode

                return true;
            }
            else
            {
                return false;
            }

        }
        else if (actionMode == ActionMode.item)
        {
            Debug.Log("Going for item.");
            //CancelActions();
            _targetingscan.SetTargetedEntity();
            isPickingUp = true;
            return true;
        }
        else
        {
            return false;
        }
        


    }

    public void ToggleEngageMode()
    {
        if (actionMode == ActionMode.combat)
        {
            actionMode = ActionMode.item;
            
        }
        else if (actionMode == ActionMode.item)
        {
            actionMode = ActionMode.combat;
        }

        _targetingscan.SetScanMode(actionMode);


    }

    public void SetActionMode(ActionMode mode)
    {
        actionMode = mode;
        _targetingscan.SetScanMode(actionMode);
    }



    public void CancelEngage()
    {
        isEngaging = false;
        isInFormation = true;
        isReturning = true;
        isIncanting = false;
        OnIncantFocusChanged?.Invoke();

        //turn off targettingscan but activate target arrow to core
        _targetingscan.targeted_entity = core_obj;
        _targetingscan.ActivateTargetArrow();


    }


    public void CancelActions()
    {
        isEngaging = false;
        isPickingUp = false;
        isPerusing = false;
        isInFormation = true;
        isReturning = true;
        isIncanting = false;
        
        OnIncantFocusChanged?.Invoke();

        //turn off targettingscan but activate target arrow to core
        _targetingscan.scanningOn = false;
        _targetingscan.targeted_entity = core_obj;
        _targetingscan.ActivateTargetArrow();


    }


    public void SetIsInFormation(bool formationState)
    {
        isInFormation = formationState;
    }

    public void SetCharacterControllerState(bool controllerState)
    {
        _controller.enabled = controllerState;
    }

    public void SetNavMeshAgentState(bool nvmaState)
    {
        _navMeshAgent.enabled = nvmaState;
    }

    public void SetTargettingArrowState(bool arrowState)
    {
        _targetingscan.target_arrow_on = arrowState;
    }

    private void FollowFormation()
    {
        core_homing_vector = (_slotprojector.slot_array[slot_num] - transform.position);

        RaycastHit hitinfo;
        if (!Physics.SphereCast(transform.position, _entityStats.entity_radius, core_homing_vector, out hitinfo, core_homing_vector.magnitude))
        {
            _navMeshAgent.enabled = false;
            _controller.enabled = true;

            Vector3 look_direction = core_obj.transform.forward;
            if (look_direction != Vector3.zero && !isEngaging) // Avoid errors if direction is zero
            {
                Quaternion lookRotation = Quaternion.LookRotation(look_direction);

                // Smoothly interpolate from current rotation to target rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10 * Time.deltaTime);
            }

            //transform.rotation = core_obj.transform.rotation;

            Vector3 ch_velocity = Mathf.Clamp(core_homing_vector.magnitude, 0f, 1.0f) * core_homing_vector.normalized * _entityStats.running_speed;

            if (_controller.isGrounded)
            {
                ch_velocity.y = -2f; // Small negative value to "stick" to ground
            }
            else
            {
                ch_velocity.y += -9.81f * Time.deltaTime; // Apply gravity
            }


            _controller.Move(ch_velocity* Time.deltaTime);


        }
        else
        {
            _controller.enabled = false;
            _navMeshAgent.enabled = true;
            _navMeshAgent.destination = _slotprojector.slot_array[slot_num];

        }

        if (isReturning)
        {
            if (Vector3.Distance(core_obj.transform.position, transform.position) <= _slotprojector.squad_distance * 2)
            {
                isReturning = false;
                _targetingscan.DeactivateTargetArrow();
                _targetingscan.targeted_entity = null;

            }
        }

    }

    public void SetSkillPerforming(Skill_SO skill)
    {
        skill_performing = skill;
    }

    private void FaceTarget(GameObject target_object)
    {
        Vector3 target_direction = target_object.transform.position - transform.position;
        target_direction.y = 0;

        if (target_direction != Vector3.zero) // Avoid errors if direction is zero
        {
            Quaternion targetRotation = Quaternion.LookRotation(target_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }

    IEnumerator WaitforCastingToComplete(float castingTime)
    {
        float waitTime = castingTime / (1 + StatScale(_entityStats.int_adjusted));
        Debug.Log("casting wait time = " + waitTime);

        yield return new WaitForSeconds(waitTime);
        magicCompleted = true;
    }

    IEnumerator MeleeAttackCooldown(float meleeTime)
    {
        yield return new WaitForSeconds(meleeTime);

        isMeleeAttacking = false; // Reset the cooldown flag
    }


    IEnumerator RangedAttackCooldown(float rangedCooldown)
    {

        float waitTime = rangedCooldown / (1 + StatScale(_entityStats.dex_adjusted));
        Debug.Log("range wait time = " + waitTime);

        yield return new WaitForSeconds(waitTime);
        isRangedAttacking = false;
    }

    public bool CheckTargetAvailable()
    {
        if (_targetingscan.targeted_entity == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    private float StatScale(float stat)
    {
        return (stat - 50f) / 50f;
    }
}
