using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "DefaultBehavior_SO", menuName = "Enemy Behavior/DefaultBehavior_SO")]
public abstract class DefaultBehavior_SO : ScriptableObject
{
   
    public abstract void Perform(GameObject entity_obj, NavMeshAgent _navMeshAgent);
}
