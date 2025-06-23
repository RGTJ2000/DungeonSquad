using UnityEngine;

[CreateAssetMenu(fileName = "ModifyStatEffect_SO", menuName = "Potion Effects/ModifyStatEffect_SO")]
public class ModifyStatEffect_SO : PotionEffect_SO
{
    public StatType statToModify;
    public int amount;

    public override void Execute(GameObject target)
    {
        EntityStats _entityStats = target.GetComponent<EntityStats>();
        if (_entityStats == null)
        {
            Debug.LogWarning("Target does not have CharacterStats component.");
            return;
        }

        switch (statToModify)
        {
            case StatType.strength:
                _entityStats.strength += amount;
                break;
            case StatType.dexterity:
                _entityStats.dexterity += amount;
                break;
            case StatType.intelligence:
                _entityStats.intelligence += amount;
                break;
            case StatType.will:
                _entityStats.will += amount;
                break;
            case StatType.soul:
                _entityStats.soul += amount;
                break;
            case StatType.maxHealth:
                _entityStats.health_max += amount;
                break;
            default:
                Debug.LogWarning("Unhandled stat type: " + statToModify);
                break;
        }

        _entityStats.UpdateAdjustedStats();

        Debug.Log($"Modified {statToModify} by {amount} on {target.name}");
    }


}
