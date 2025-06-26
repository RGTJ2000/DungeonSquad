using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells/CastSpell_SO")]

public class CastSpell_SO : Skill_SO
{
    /*
    [Header("General Stats")]
    public float castingTime;
    public Vector3 castingOffsetfromCaster;
    //public float hitChanceMultiplier;
    public bool alwaysHit;
    */
    

    [Header("Spell Effect")]
    [SerializeField] private Skill_SO spellEffect;

    public override void Use(GameObject attacker, GameObject target)
    {
        SkillCooldownTracker _cooldownTracker = attacker.GetComponent<SkillCooldownTracker>();

        if (target != null )
        {
            NavMeshAgent _navMeshAgent = attacker.GetComponent<NavMeshAgent>();
           
            Vector3 target_direction = (target.transform.position - attacker.transform.position).normalized;
            target_direction.y = 0;

            _navMeshAgent.transform.position = attacker.transform.position;
            FaceTarget(attacker, target_direction);

            if (Vector3.Angle(attacker.transform.forward, target_direction) < 10f)
            {
                if (_cooldownTracker != null && _cooldownTracker.GetRemainingCooldown(this) == 0)
                {
                    if (spellEffect != null)
                    {
                        EntityStats _entityStats = attacker.GetComponent<EntityStats>();

                        float _cooldown = cooldown / (1 + StatScale(_entityStats.int_adjusted));
                        Debug.Log("Cooldown for " + this.skill_name + " set to " + _cooldown);
                        spellEffect.Use(attacker, target);
                        _cooldownTracker.StartCooldown(this, _cooldown);
                    }

                }


            }


        }

        if (_cooldownTracker.GetRemainingCooldown(this) > 0 && _cooldownTracker.GetRemainingCooldown(spellEffect) == 0)
        {

            if (attacker.TryGetComponent<Ch_Behavior>(out var ch_behavior))
            {
                ch_behavior.CancelEngage();

            }

        }






         
    }

    private void FaceTarget(GameObject origin_obj, Vector3 target_direction)
    {


        if (target_direction != Vector3.zero) // Avoid errors if direction is zero
        {
            Quaternion targetRotation = Quaternion.LookRotation(target_direction);
            origin_obj.transform.rotation = Quaternion.Slerp(origin_obj.transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }

    private float StatScale(float stat)
    {
        return (stat - 50f) / 50f;
    }
}