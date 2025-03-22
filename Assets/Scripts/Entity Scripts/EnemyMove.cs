using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMove : MonoBehaviour
{
    public bool isIdle = false;
    public bool isPatrolling = false;
    public bool isWandering = true;
    public bool isEngaging = false;
    public bool isAttacking = false;

    private Vector3 home_position = Vector3.zero;
    public Vector3 destination_position = Vector3.zero;
    private float close_enough_distance = 1.0f;

    private int max_attempts = 100;
    private float boundmax_x = 20f;
    private float boundmax_z = 20f;

    private NavMeshAgent _navMeshAgent;

    private ScanForCharacters _scanforchars;

    private Combat _combat;
    private EntityStats _entityStats;

    private bool isMeleeAttacking = false;
    private bool isRangedAttacking = false;
    private bool isMagicAttacking = false;
    private bool magicCompleted = false;

    private bool isIncanting = false;
    private GameObject currentIncantTarget = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        home_position = transform.position;
        destination_position = transform.position;
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _scanforchars = GetComponent<ScanForCharacters>(); 
        _combat = GetComponent<Combat>();
        _entityStats = GetComponent<EntityStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWandering)
        {

            if (_scanforchars.SetAndReturnNearestCharacter(_entityStats.visible_distance) != null)
            {
                isWandering = false;
                isEngaging = true;
            }
            else //if no targetted_character continue wandering behavior
            {
                PerformWander();
            }
        } else if (isEngaging)
        {
            if (_scanforchars.targeted_character != null)
            {
                if (_navMeshAgent.enabled)
                {
                    ActivePursuit(_scanforchars.targeted_character);

                }

            }
            else
            {
                if (_scanforchars.SetAndReturnNearestCharacter(_entityStats.visible_distance) == null)
                { 
                    CancelEngage();
                }
            }


        }


    }

    private void ActivePursuit(GameObject obj_to_pursue)
    {
        _navMeshAgent.speed = _entityStats.running_speed;


        RaycastHit hit;
        Vector3 direction = (obj_to_pursue.transform.position - transform.position).normalized;
        float maxDistance = _entityStats.equipped_meleeWeapon.melee_reach + _entityStats.entity_radius;

        if (Physics.Raycast(transform.position, direction, out hit, maxDistance))
        {
            if (hit.transform == obj_to_pursue.transform)
            {
                _navMeshAgent.destination = transform.position;
                if (!isMeleeAttacking)
                {
                    isMeleeAttacking = true;

                    Skill_SO skill = _entityStats.selected_skill;
                    Debug.Log("Skill to use:" + skill.skill_name);
                    skill.Use(gameObject, obj_to_pursue);

                    StartCoroutine(MeleeAttackCooldown(_entityStats.equipped_meleeWeapon.attackCooldown));
                }
            }
        }
        else
        {
            _navMeshAgent.destination = obj_to_pursue.transform.position;
        }


     
        
      }

   private void CancelEngage()
    {
        isEngaging = false;
        isWandering = true;
        isIncanting = false;
       
    }

    private void PerformWander()
    {
        //Check if at destination, if so get new random destination
        if (Vector3.Distance(transform.position, destination_position) < close_enough_distance)
        {
            for (int i = 0; i < max_attempts; i++)
            {
                Vector3 random_position = new Vector3(Random.Range(-boundmax_x, boundmax_x), 0, Random.Range(-boundmax_z, boundmax_z));
                if (!Physics.CheckSphere(random_position + Vector3.up, 0.5f))
                {
                    destination_position = random_position;
                    break;
                }
            }
        }

        _navMeshAgent.speed = _entityStats.walking_speed;
        _navMeshAgent.destination = destination_position;
    }

    public void CheckAndSetAttacker(GameObject attacker)
    {
        if (attacker != _scanforchars.targeted_character)
        {
            _scanforchars.ChangeTargetedCharacter(attacker);
        }
    }


    IEnumerator MeleeAttackCooldown(float meleeTime)
    {
        yield return new WaitForSeconds(meleeTime);

        isMeleeAttacking = false; // Reset the cooldown flag
    }
}
