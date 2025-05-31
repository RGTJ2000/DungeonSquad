using UnityEngine;
using System.Collections;

public class MagicHandler : MonoBehaviour
{

    private EntityStats _entityStats;
   

    public GameObject magicMissile_prefab;
    public GameObject fireball_prefab;

    private float mm_castingTime = 1.0f;
    private Vector3 mm_castingOffsetVector = new Vector3(0f,0f,0.5f);
    private float mm_damageBase = 50f;
    private float mm_damageRange = 10.0f;
    private float mm_hitChanceMultiplier = 10f; //magic missile spell increases hit chance dramatically

    public bool magic_completed;
    public bool magic_isCasting;

    private Vector3 fb_castingOffsetVector = new Vector3(0f, 0.5f, 0.5f);
    private float fb_castingTime = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _entityStats = GetComponent<EntityStats>();
        magic_completed = false;
    }


    public void CastActiveSpell(GameObject target)
    {
        switch (_entityStats.selected_skill.skill_name)
        {
            case "Magic Missile": //magic missile
                StartCoroutine(CastMagicMissile(target));
                break;
            case "Fireball": //fireball
                 StartCoroutine(CastFireball(target));
                break;
            default:
                break;

        }

    }

    IEnumerator CastMagicMissile(GameObject mmTarget)
    {
        Vector3 instantiatePoint = transform.position + (transform.forward * _entityStats.entity_radius) + ( Quaternion.LookRotation(transform.forward, Vector3.up) * mm_castingOffsetVector );
        Debug.DrawLine(instantiatePoint, instantiatePoint+Vector3.up *3, Color.red, 2.0f);
       
        GameObject magicMissile = Instantiate(magicMissile_prefab, instantiatePoint, transform.rotation);
        MM_Guidance _mmGuidance2 = magicMissile.GetComponent<MM_Guidance>();
        //_mmGuidance2.SetMMParameters(gameObject, mmTarget, mm_castingTime, 200f, mm_damageBase, mm_damageRange, DamageType.physical, mm_hitChanceMultiplier, _entityStats.magic_attackRating);
       
        yield return new WaitForSeconds(mm_castingTime);

        magic_completed = true;

    }

    IEnumerator CastFireball(GameObject fireballTarget)
    {
        Vector3 instantiatePoint = transform.position + (transform.forward * _entityStats.entity_radius) + (Quaternion.LookRotation(transform.forward, Vector3.up) * fb_castingOffsetVector);

        Debug.Log("Casting Fireball");
        yield return new WaitForSeconds(fb_castingTime);
        Debug.Log("Firebal cast complete.");
        magic_completed = true;
    }

}
