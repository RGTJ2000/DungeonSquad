using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class MM_Guidance2 : MonoBehaviour
{
    private GameObject missile_target;

    private GameObject origin_obj = null;
    private float damage_base = 0f;
    private float damage_range = 0f;
    private float caster_magicAR;
    private string targetTag;


    private TargetingScan_General _targetingScanGeneral;
    private CapsuleCollider _capsuleCollider;



    private Vector3 heading;
    private Vector3 rise_target;


    private float mm_acc = 200f;
    private float magic_hitChanceMultiplier = 100f;

    private Rigidbody _rb;
    //private ParticleSystem _particleSystem;




    private Vector3 startPoint;
    private Vector3 riseOffset;
    public float riseDuration = 10f;
    private float launchPause = 0.1f;

    private bool launchStarted = false;
    private bool seekTarget = false;

    private bool hasCollided;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _targetingScanGeneral = GetComponent<TargetingScan_General>();
        _capsuleCollider = GetComponent<CapsuleCollider>();


        startPoint = transform.position;
        riseOffset = Vector3.up * 4f;
        rise_target = startPoint + riseOffset;

        hasCollided = false;

        //Debug.Log("MM guidance 2: Missile launched");

    }


    private void FixedUpdate()
    {

        if (missile_target != null)
        {
            heading = (missile_target.transform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(heading.normalized, Vector3.up);

            if (!launchStarted)
            {
                StartCoroutine(RiseToLaunchHeight());
            }
            else if (seekTarget)
            {
                AccelerateToTarget();
            }

        }
        else
        {
            //find new target
            missile_target = _targetingScanGeneral.FindNearestTarget(targetTag);

            if (missile_target == null)
            {
                StartCoroutine(DeactivateDestroyMissile());
            }
        }


    }

    private void OnCollisionEnter(Collision collision)
    {

        GameObject collided_obj = collision.gameObject;
        if (!hasCollided)
        {
            SoundManager.Instance.PlayMMBoom();
            StartCoroutine(DeactivateDestroyMissile());
            hasCollided = true;


            if (collided_obj.tag == "Enemy" || collided_obj.tag == "Character")
            {
                CombatManager.Instance.ResolveMagic(origin_obj, collided_obj, "physical", damage_base, damage_range, magic_hitChanceMultiplier, caster_magicAR);

            }


        }



    }

    public void SetMMParameters(GameObject originObj, GameObject target, float castTime, float acc, float _damageBase, float _damageRange, float mm_hitChanceMultiplier, float magicAR)
    {
        Debug.Log("Magic Missile target: " + target.name);
        origin_obj = originObj;
        missile_target = target;
        riseDuration = castTime;
        mm_acc = acc;
        damage_base = _damageBase;
        damage_range = _damageRange;
        magic_hitChanceMultiplier = mm_hitChanceMultiplier;
        caster_magicAR = magicAR;

        EntityStats _entityStats = originObj.GetComponent<EntityStats>();
        string type = _entityStats.selected_skill.skill_targetType;

        {
            if (originObj.tag == "Character")
            {
                switch (type)
                {
                    case "self":
                        break;
                    case "group":
                        targetTag = "Character";
                        break;
                    case "other":
                        targetTag = "Enemy";
                        break;
                    case "area":
                        break;
                    default:
                        targetTag = "none";
                        break;

                }

            }
            else if (originObj.tag == "Enemy")
            {
                switch (type)
                {
                    case "self":
                        break;
                    case "group":
                        targetTag = "Enemy";
                        break;
                    case "other":
                        targetTag = "Character";
                        break;
                    case "area":
                        break;
                    default:
                        targetTag = "none";
                        break;

                }

            }



        }
    }

        IEnumerator RiseToLaunchHeight()
        {
            launchStarted = true;
            _rb.isKinematic = true;
            _capsuleCollider.enabled = false;
            float start_time = Time.time;
            Vector3 current_velocity = Vector3.zero;

            //SoundManager.Instance.PlayMMLaunch();

            while ((Time.time - start_time) < riseDuration && (transform.position - rise_target).magnitude > 0.1f)
            {
                transform.position = Vector3.SmoothDamp(transform.position, rise_target, ref current_velocity, riseDuration);
                yield return null;
            }

            SoundManager.Instance.PlayMMLaunch();

            yield return new WaitForSeconds(launchPause);


            _rb.isKinematic = false;
            _capsuleCollider.enabled = true;

            seekTarget = true;

        }

        private void AccelerateToTarget()
        {
            _rb.linearVelocity = (_rb.linearVelocity.magnitude + mm_acc * Time.deltaTime) * heading.normalized;
            //_rb.linearVelocity = heading.normalized * 40f;
            //Debug.Log("heading:" + heading.normalized * 10f);
        }



        public void SetTarget(GameObject target)
        {
            missile_target = target;
        }

        IEnumerator DeactivateDestroyMissile()
        {
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<TrailRenderer>().enabled = false;

            yield return new WaitForSeconds(0.2f); //let spark emit for a bit

            //stop emission
            GameObject particle_obj = transform.GetChild(0).gameObject;
            GameObject sparks_obj = particle_obj.transform.GetChild(0).gameObject;
            ParticleSystem _particleSystem = sparks_obj.GetComponent<ParticleSystem>();
            var emission = _particleSystem.emission;
            emission.enabled = false;
            //stop light
            particle_obj.transform.GetChild(1).gameObject.SetActive(false);

            yield return new WaitForSeconds(1f); //wait for sparks to die out
            Destroy(gameObject);

        }

    }
