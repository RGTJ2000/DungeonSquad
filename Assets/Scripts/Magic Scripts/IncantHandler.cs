using UnityEngine;

public class IncantHandler : MonoBehaviour
{
    private EntityStats _entityStats;

    public GameObject healing_prefab;

    private float healing_rate = 1.0f; //health points per second
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _entityStats = GetComponent<EntityStats>();
        
    }


    public void CastActiveIncant(GameObject target)
    {
        switch (_entityStats.selected_skill.skill_name)
        {
            case "Heal": //healing
                GameObject healing_obj = Instantiate(healing_prefab, target.transform.position, Quaternion.identity);

                HealingObjScript _healingObjScript = healing_obj.GetComponent<HealingObjScript>();
                _healingObjScript.SetHealingObjParameters(target, healing_rate, gameObject);
                Debug.Log("INCANT HEALING OBJ CAST!");
                break;

        }

    }


}
