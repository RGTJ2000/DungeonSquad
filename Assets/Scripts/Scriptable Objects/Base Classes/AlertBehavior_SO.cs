using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "AlertBehavior_SO", menuName = "Enemy Behavior/AlertBehavior_SO")]
public abstract class AlertBehavior_SO : ScriptableObject
{
    public abstract void Perform(GameObject enemy_obj, ScanForCharacters _scanForCharacter, NavMeshAgent _navMeshAgent, EntityStats _entityStats, float range);
}
