using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static SoundManager;

public class MissileLauncher : MonoBehaviour
{

    

    private EntityStats _entityStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _entityStats = GetComponent<EntityStats>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LaunchMissile(GameObject target_obj, GameObject origin_obj)
    {
        GameObject missile; //the missile to be instantiated
        EntityStats _attackerStats = origin_obj.GetComponent<EntityStats>();

        Ranged_Weapon_SO rangedWeapon = _attackerStats.equipped_rangedWeapon.RangedWeapon;
        Missile_SO equippedMissile = _attackerStats.equipped_missile.Missile;

        //Missile_SO missileSO = _attackerStats.equipped_missile.baseItem as Missile_SO;

        GameObject missile_prefab = equippedMissile.missileLaunch_prefab;

        float missileSpeed = rangedWeapon.launch_impulse / equippedMissile.missile_weight;


        //calculate orientation and launch vector
        Vector3 launch_vector = CalculateLaunchVector(target_obj, missileSpeed);

        launch_vector = RandomizeLaunchAccuracy(launch_vector);

        missile = Instantiate(missile_prefab, transform.position, transform.rotation);

        BoxCollider missileCollider = missile.GetComponent<BoxCollider>();
        CapsuleCollider entityCollider = GetComponent<CapsuleCollider>();
        missileCollider.enabled = false;

        //MISSILE CRIT CHANCE
        float missileCritChance = rangedWeapon.ranged_critBase * (1 + equippedMissile.missile_critBaseModifier) * (1 + (2*_attackerStats.ranged_critBonusFactor));

        //Debug.Log("Ranged Weapon crit base=" + rangedWeapon.ranged_critBase + "  missile crit modifier=" + equippedMissile.missile_critBaseModifier + "  attacker ranged crit mod=" + _attackerStats.ranged_critBonusFactor);

        MissileData missileData = new MissileData(origin_obj, target_obj, _attackerStats.ranged_attackRating, missileCritChance, equippedMissile.damageList);

        //Debug.Log("Sending missileData to Prefab. --attacker:"+origin_obj.name+" --target:"+target_obj.name+" --attacker_AR="+_attackerStats.ranged_attackRating+"  --missileCritChance="+missileCritChance);

        missile.GetComponent<MissileGuidance>().SetMissileParameters(missileData, launch_vector, missileSpeed);

        missile.transform.LookAt(target_obj.transform, Vector3.up);


        if (missileCollider != null && entityCollider != null)
        {
            Physics.IgnoreCollision(missileCollider, entityCollider, true);
        }
        StartCoroutine(EnableCollider(missileCollider));


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

        float degrees_offtarget = (_entityStats.degrees_of_accuracy / 2) * Random.Range(-1.0f, 1.0f); //degrees missile can be offtarget
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
