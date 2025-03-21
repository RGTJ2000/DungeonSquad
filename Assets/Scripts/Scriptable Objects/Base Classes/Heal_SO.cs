using UnityEngine;

[CreateAssetMenu(fileName = "New Heal_SO", menuName = "Incants/Heal_SO")]
public class Heal_SO : Skill_SO
{
    public float healingRate;
    public GameObject healing_prefab;

    public override void Use(GameObject incanter, GameObject target)
    {
        GameObject healing_obj = Instantiate(healing_prefab, target.transform.position, Quaternion.identity);

        HealingObjScript _healingObjScript = healing_obj.GetComponent<HealingObjScript>();
        _healingObjScript.SetHealingObjParameters(target, healingRate, incanter);

    }

}
