using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static SoundManager;

public class MissileLauncher : MonoBehaviour
{

    public float degrees_of_accuracy = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LaunchMissile(GameObject target_obj, GameObject origin_obj)
    {
        GameObject missile; //the missile to be instantiated
        EntityStats _attackerStats = origin_obj.GetComponent<EntityStats>();
        RuntimeItem missile_rt_SO = _attackerStats.equipped_missile;
        GameObject missile_prefab = missile_rt_SO.item_prefab;

        float missileSpeed = _attackerStats.equipped_rangedWeapon.RangedWeapon.launch_impulse / missile_rt_SO.Missile.missile_weight;

        //calculate orientation and launch vector
        Vector3 launch_vector = CalculateLaunchVector(target_obj, missileSpeed);

        launch_vector = RandomizeLaunchAccuracy(launch_vector);

       
       

        //missile = Instantiate(missile_prefab, transform.position+transform.forward*_attackerStats.entity_radius, transform.rotation);

        missile = Instantiate(missile_prefab, transform.position, transform.rotation);

        BoxCollider missileCollider = missile.GetComponent<BoxCollider>();
        CapsuleCollider entityCollider = GetComponent<CapsuleCollider>();
        missileCollider.enabled = false;

        missile.GetComponent<MissileGuidance>().SetMissileParameters(origin_obj, launch_vector, missile_rt_SO, missileSpeed);

        missile.transform.LookAt(target_obj.transform, Vector3.up);


        if (missileCollider != null && entityCollider != null)
        {
            Physics.IgnoreCollision(missileCollider, entityCollider, true);
        }
        StartCoroutine(EnableCollider(missileCollider));


        //missile.GetComponent<MissileGuidance>().SetMissileParameters(origin_obj, launch_vector, damageBase, damageRange, missileSpeed, missileRange);


        Debug.Log("Playing bowlaunch");
        string soundID = _attackerStats.equipped_rangedWeapon.RangedWeapon.launchAudio_ID;
        SoundManager.Instance.PlayVariationAtPosition(soundID, transform.position, SoundCategory.sfx);

    }

    private Vector3 CalculateLaunchVector(GameObject target_obj, float speed)
    {
        float angle_degrees;

        Vector3 directLineVector = (target_obj.transform.position - transform.position);
        Vector3 heading_vector = Vector3.ProjectOnPlane(directLineVector, Vector3.up);  //projects the direct line vector onto the ground plane

        // Calculate the horizontal distance and height difference
        float distance = heading_vector.magnitude;
        float height = target_obj.transform.position.y - transform.position.y; // Vertical difference

        // Calculate the required launch angle using the quadratic formula
        float g = Physics.gravity.magnitude; // Unity's gravity value
        float speedSquared = speed * speed;
        float discriminant = 1 - (2 * g * height / speedSquared) - ((g * g * distance * distance) / (speedSquared * speedSquared));

        // Check if the target is reachable
        if (discriminant < 0)
        {
            //use 45 degrees to go longest distance possible
            angle_degrees = 45f;
        }
        else
        {
            float sqrtDiscriminant = Mathf.Sqrt(discriminant);
            angle_degrees = Mathf.Atan((speedSquared / (g * distance)) * (1 - sqrtDiscriminant)) * Mathf.Rad2Deg; //subtracts discriminant to get lower launch angle
        }



        // Rotate heading_vector up by angle_degree
        Vector3 rotation_axis = Vector3.Cross(heading_vector, Vector3.up);

        Vector3 launch_vector = Quaternion.AngleAxis(angle_degrees, rotation_axis) * heading_vector.normalized;


        return launch_vector;
    }

    private Vector3 RandomizeLaunchAccuracy(Vector3 original_vector)
    {
        Vector3 randomized_vector;

        float degrees_offtarget = (degrees_of_accuracy / 2) * Random.Range(-1.0f, 1.0f); //degrees missile can be offtarget
        //Debug.Log($"DEGREES OFF TARGET = {degrees_offtarget}");

        Vector3 random_rotation_axis = Vector3.Cross(original_vector, Vector3.up).normalized;
        random_rotation_axis = Quaternion.AngleAxis(Random.Range(360f, 0f), original_vector.normalized) * random_rotation_axis;

        //Debug.Log($"degrees off: {degrees_offtarget} - rotation axis: {random_rotation_axis}");

        randomized_vector = Quaternion.AngleAxis(degrees_offtarget, random_rotation_axis) * original_vector.normalized;
        //Debug.Log($"original vector: {original_vector} -- randomized_vector: {randomized_vector}");

        return randomized_vector;
    }

    IEnumerator EnableCollider(Collider col)
    {
        yield return null; // Wait one frame
        if (col != null)
        {
            col.enabled = true;
        }
    }
}
