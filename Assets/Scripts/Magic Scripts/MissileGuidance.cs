using System.Drawing;
using UnityEngine;

public class MissileGuidance : MonoBehaviour

{
    private GameObject origin_obj;
    private float missile_speed;
    private Vector3 heading_vector;
    private float damage_base;
    private float damage_range;
    private float missile_AR;
    private float missile_critChance;
    private Missile_SO missile_SO;

    //private Vector3 launch_origin;

    private Rigidbody _rb;
    private FixedJoint _joint;
    private BoxCollider _boxCollider;

    private bool inFlight = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //launch_origin = transform.position;
        _rb = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {

       


    }

    private void FixedUpdate()
    {

        if (inFlight)
        {
            AlignMissileWithVelocity();

        }



    }
    private void OnCollisionEnter(Collision collision)
    {
        
        GameObject collided_obj = collision.gameObject;

        if (collided_obj != origin_obj)
        {
            inFlight = false;

            Vector3 inFlightDirection = _rb.linearVelocity;

            _rb.linearVelocity = Vector3.zero; // Stop movement
            _rb.angularVelocity = Vector3.zero; // Stop rotation
                                                //_rb.useGravity = false; // Optional, depending on your needs
            _rb.isKinematic = true;

           

            if (collided_obj.CompareTag("Enemy") || collided_obj.CompareTag("Character"))
            {
                // Get the contact point and normal from the collision
                ContactPoint contact = collision.contacts[0];
                Vector3 surfaceNormal = contact.normal; // Points away from the enemy surface

                Vector3 midwayDirection = Vector3.Slerp(inFlightDirection, -surfaceNormal, 0.3f).normalized;

                // Align the arrow perpendicular to the surface
                transform.forward = midwayDirection; // Arrow points outward from enemy
                                                     // If your arrow model points the wrong way, use: transform.forward = -surfaceNormal;

                // Optional: Position the arrow at the contact point
                transform.position = contact.point; // Snaps arrow to exact hit location


                transform.SetParent(collided_obj.transform);
                
                CombatManager.Instance.ResolveMissile(origin_obj, collided_obj, missile_SO, missile_AR, missile_critChance);
            }
            else
            {
                

                
                // Non-enemy collision: still add joint but no rotation adjustment
                _joint = gameObject.AddComponent<FixedJoint>();
                _joint.connectedBody = collision.rigidbody;
                _joint.breakForce = Mathf.Infinity;
                

            }

            _boxCollider.enabled = false;
            Destroy(gameObject, 1.0f);


        }
       
    }


    public void SetMissileParameters(GameObject originObj, Vector3 heading, Missile_SO missile, float speed)
    {
        inFlight = true;
        origin_obj = originObj;
        missile_SO = missile;
        heading_vector = heading;

        

        damage_base = missile_SO.missile_damageBase;
        damage_range = missile_SO.missile_damageRange;

        EntityStats _attackerStats = origin_obj.GetComponent<EntityStats>();

        //set missile AR and critChance (need to calculate critChance here in case attacker changes weapons or dies while missile in flight)
        missile_AR = _attackerStats.ranged_attackRating;
        missile_critChance = CalculateCritChance(_attackerStats);

        //set missile velocity
        missile_speed = speed;
        Rigidbody _rb = GetComponent<Rigidbody>();
        if (_rb != null)
        {
            _rb.linearVelocity = heading_vector.normalized * missile_speed;

            
        }


    }

    private void AlignMissileWithVelocity()
    {
        // Ensure the Rigidbody and velocity are valid
        if (_rb != null && _rb.linearVelocity != Vector3.zero)
        {
            // Calculate the desired rotation to align with velocity
            Quaternion targetRotation = Quaternion.LookRotation(_rb.linearVelocity.normalized);
            // Apply the rotation using Rigidbody.MoveRotation
            _rb.MoveRotation(targetRotation);

        }

    }

    private float CalculateCritChance(EntityStats attackerStats)
    {
        float critChance = 0f;

        float str_w = attackerStats.equipped_meleeWeapon.critChance_weight_str;
        float dex_w = attackerStats.equipped_meleeWeapon.critChance_weight_dex;
        float int_w = attackerStats.equipped_meleeWeapon.critChance_weight_int;
        float will_w = attackerStats.equipped_meleeWeapon.critChance_weight_will;
        float soul_w = attackerStats.equipped_meleeWeapon.critChance_weight_soul;

        float STR = attackerStats.strength;
        float DEX = attackerStats.dexterity;
        float INT = attackerStats.intelligence;
        float WLL = attackerStats.will;
        float SOL = attackerStats.soul;

        critChance = attackerStats.equipped_meleeWeapon.critChance_base * (1 + ((CritCalc(str_w, STR) + CritCalc(dex_w, DEX) + CritCalc(int_w, INT) + CritCalc(will_w, WLL) + CritCalc(soul_w, SOL)) / (str_w + dex_w + int_w + will_w + soul_w)));

        return critChance;
    }
    private float CritCalc(float weight, float stat)
    {
        float calc = weight * ((stat - 50) / 50);
        return calc;
    }
}
