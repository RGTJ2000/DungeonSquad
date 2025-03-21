using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Progress;

public class Fireball_Guidance : MonoBehaviour
{
    private GameObject caster;
    private GameObject target;

    private float startSize;

    private float cast_duration;
    private float fbSpeed;
    private float contact_dBase;
    private float contact_dRange;
    private float blastDiameter;
    private float blastRadius;
    private float blastImpulse;
    private float blastSpeed;
    private float blast_dBase;
    private float blast_dRange;

    private float magic_hitChanceMultiplier = 1f;
    private float caster_magicAR;


    private Vector3 heading;
    private Vector3 endPosition;

    private int castingStage;


    private Vector3 startScale;
    private Vector3 endScale;
    private float elapsedTime = 0f; // Time since scaling started

    private bool explosionBegun = false;

    private float stunTime = .2f;
    private bool disableStarted = false;

    private struct explodedEntityInfo
    {
        public GameObject entity_obj;
        public Vector3 moveDirection;
        public float dModifier;

    }

    private List<explodedEntityInfo> explodedList = new List<explodedEntityInfo>();

    void Start()
    {
        castingStage = 0;
        SoundManager.Instance.PlayFireLoop(() => castingStage != 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (castingStage == 0)
        {
            GrowFireball();
        }
        else if (castingStage == 1)
        {
            TravelFireball();
        }
        else if (castingStage == 2)
        {
            ExplodeFireball();
        }
       

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target || other.CompareTag("Wall"))
        {
            castingStage = 2;
        }
        else if (castingStage != 2)
        {
            if (other.CompareTag("Enemy") || other.CompareTag("Character"))
            {
                CombatManager.Instance.ResolveMagic(caster, other.gameObject, "fire", contact_dBase, contact_dRange, magic_hitChanceMultiplier, caster_magicAR);
            }

        }


    }

    private void GrowFireball()
    {
        if (elapsedTime <= cast_duration)
        {
            float t = Mathf.Clamp01(elapsedTime / cast_duration); // 0 to 1 over duration
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsedTime += Time.deltaTime;
        }
        else
        {
            transform.localScale = endScale;
            castingStage = 1;
        }

    }

    private void TravelFireball()
    {
        if (Vector3.Distance(transform.position, endPosition) < 0.1f)
        {
            castingStage = 2;
        }
        else
        {
            transform.position += heading * fbSpeed * Time.deltaTime;
        }
    }

    private void ExplodeFireball()
    {
        if (!explosionBegun && transform.localScale.x > 0.1f)
        {
            transform.localScale = transform.localScale - (Vector3.one * blastSpeed * Time.deltaTime);
        }
        else if (!explosionBegun)
        {
            explosionBegun = true;
            SoundManager.Instance.PlayFireballBoom();
            DamagAreaMakeList();

        }

        if (explosionBegun && castingStage == 2)
        {
            transform.localScale = transform.localScale + (Vector3.one * blastSpeed * Time.fixedDeltaTime); //expand blast

            if (Mathf.Abs(transform.localScale.x) >= blastDiameter)
            {
                castingStage = 3; //End explosion
                if (!disableStarted)
                {
                    Debug.Log("Calling Subroutine");

                    StartCoroutine(DisableImpulseDestroy());

                }
                disableStarted = true;

            }
        }

    }

    IEnumerator DisableImpulseDestroy()
    {
        ParticleSystem _pSystem = GetComponent<ParticleSystem>();
        _pSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        GameObject effectsContainer = transform.GetChild(0).gameObject;
        effectsContainer.SetActive(false);

        if (explodedList.Count > 0)
        {
            foreach (explodedEntityInfo info in explodedList)
            {
                if (info.entity_obj != null)
                {
                    Rigidbody _rb = info.entity_obj.GetComponent<Rigidbody>();
                    _rb.isKinematic = false;
                    NavMeshAgent _navMeshAgent = info.entity_obj.GetComponent<NavMeshAgent>();
                    _navMeshAgent.enabled = false;
                    Debug.Log("Adding impulse " + blastImpulse + " to " + info.entity_obj.name);
                    _rb.AddForce(info.moveDirection * blastImpulse * info.dModifier, ForceMode.Impulse);
                }
               
            }
        }

        yield return new WaitForSeconds(stunTime);

        if (explodedList.Count > 0)
        {
            foreach (explodedEntityInfo info in explodedList)
            {
                if (info.entity_obj != null)
                {
                    Rigidbody _rb = info.entity_obj.GetComponent<Rigidbody>();
                    _rb.isKinematic = true;
                    NavMeshAgent _nmAgent = info.entity_obj.GetComponent<NavMeshAgent>();
                    _nmAgent.enabled = true;
                }
            }
        }

        Destroy(gameObject);



    }

    public void SetParameters(GameObject caster_obj, GameObject target_obj, float size, float duration, float travelSpeed, float contactDB, float contactDR, float diameter, float impulse, float bSpeed, float blastDB, float blastDR, float magicAR)
    {
        caster = caster_obj;
        target = target_obj;
        cast_duration = duration;
        fbSpeed = travelSpeed;
        contact_dBase = contactDB;
        contact_dRange = contactDR;
        blastDiameter = diameter;
        blastImpulse = impulse;
        blastSpeed = bSpeed;
        blast_dBase = blastDB;
        blast_dRange = blastDR;
        startSize = size;

        blastRadius = blastDiameter / 2;
        endPosition = target_obj.transform.position;
        heading = (endPosition - transform.position).normalized;
        startScale = transform.localScale;
        endScale = Vector3.one * size;

        caster_magicAR = magicAR;
    }

    private void DamagAreaMakeList()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {

                Vector3 distanceVector = (hit.transform.position - transform.position);
                float distModifier = (1 - Mathf.Clamp01(distanceVector.magnitude / blastRadius));

                CombatManager.Instance.ResolveMagic(caster, hit.gameObject, "fire", blast_dBase * distModifier, blast_dRange * distModifier, 1f, caster_magicAR);

                Vector3 moveVector = distanceVector;

                moveVector = new Vector3(moveVector.x, 0f, moveVector.z).normalized;

                explodedList.Add(new explodedEntityInfo { entity_obj = hit.gameObject, moveDirection = moveVector, dModifier = distModifier });
                Debug.Log("List Count = "+explodedList.Count);

            }
        }
    }
}
