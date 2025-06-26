using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "PerformIncant_SO", menuName = "Incants/PerformIncant_SO")]
public class PerformIncant_SO : Skill_SO
{
    [SerializeField] Skill_SO incantEffect;


    public override void Use(GameObject incanter, GameObject target)
    {
        SkillCooldownTracker _cooldownTracker = incanter.GetComponent<SkillCooldownTracker>();

        if (target != null)
        {
            NavMeshAgent _navMeshAgent = incanter.GetComponent<NavMeshAgent>();

            Vector3 target_direction = (target.transform.position - incanter.transform.position).normalized;
            target_direction.y = 0;

            _navMeshAgent.transform.position = incanter.transform.position;
            FaceTarget(incanter, target_direction);

            if (Vector3.Angle(incanter.transform.forward, target_direction) < 10f)
            {
                if (_cooldownTracker != null && _cooldownTracker.GetRemainingCooldown(this) == 0)
                {
                    if (incantEffect != null)
                    {
                        EntityStats _entityStats = incanter.GetComponent<EntityStats>();

                        float _cooldown = cooldown / (1 + StatScale(_entityStats.int_adjusted));

                        incantEffect.Use(incanter, target);

                        _cooldownTracker.StartCooldown(this, _cooldown);
                    }

                }


            }


        }

        if (_cooldownTracker.GetRemainingCooldown(this) > 0)
        {

            if (incanter.TryGetComponent<Ch_Behavior>(out var ch_behavior))
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
