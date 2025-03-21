using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "IdleBehavior_SO", menuName = "Enemy Behavior/IdleBehavior_SO")]
public class IdleBehavior_SO : DefaultBehavior_SO
{
    public override void Perform(GameObject entity_obj, NavMeshAgent _navMeshAgent)
    {
        if (entity_obj != null && _navMeshAgent.isOnNavMesh)
        {
            _navMeshAgent.destination = entity_obj.transform.position;
        }
    }

}
