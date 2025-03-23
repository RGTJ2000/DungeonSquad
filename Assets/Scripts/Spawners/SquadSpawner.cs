using UnityEngine;

public class SquadSpawner : MonoBehaviour
{
    [SerializeField] private GameObject core_prefab;
    [SerializeField] private GameObject fighter_prefab;
    [SerializeField] private GameObject cleric_prefab;
    [SerializeField] private GameObject wizard_prefab;
    [SerializeField] private GameObject ranger_prefab;

    private GameObject core_obj_ref;
    private GameObject[] character_ref_array;
    private Vector3 spawnPoint;

    //Get these values from a SQUAD_CONFIGDATA_SO
    private int slot_count = 4;
    private Character_Type[] slot_array = new Character_Type[4]
    {
        Character_Type.fighter,
        Character_Type.cleric,
        Character_Type.wizard,
        Character_Type.ranger,
    };

    SquadManager _squadManager;



    public void InstantiateCoreAndCharacters()
    {
        GameObject spawnPoint_obj = GameObject.Find("Ch_SpawnPoint");
        if (spawnPoint_obj == null )
        {
            Debug.Log("SpawnPoint Object not found.");
            spawnPoint = Vector3.zero;
        }
        else
        {
            spawnPoint = spawnPoint_obj.transform.position;
            Destroy(spawnPoint_obj);
        }


        //spawn core
        core_obj_ref = Instantiate(core_prefab, spawnPoint, Quaternion.identity);

        //spawn ChPrefabs, assigning them to slot positions
        character_ref_array = new GameObject[slot_count];

        for (int i = 0; i < slot_array.Length; i++)
        {
            GameObject prefabToInstantiate;

            switch (slot_array[i])
            {
                case Character_Type.fighter:
                    prefabToInstantiate = fighter_prefab;
                    break;
                case Character_Type.cleric:
                    prefabToInstantiate = cleric_prefab;
                    break;
                case Character_Type.wizard:
                    prefabToInstantiate = wizard_prefab;
                    break;
                case Character_Type.ranger:
                    prefabToInstantiate = ranger_prefab;
                    break;
                default:
                    prefabToInstantiate = null;
                    break;
            }

            // Instantiate the prefab if it's found
            if (prefabToInstantiate != null)
            {
                character_ref_array[i] = Instantiate(prefabToInstantiate, spawnPoint + Vector3.right * 2f, Quaternion.identity);

                Ch_Behavior _chBehavior = character_ref_array[i].GetComponent<Ch_Behavior>();
                _chBehavior.slot_num = i;  //this tells the character what slot to follow
                _chBehavior.core_obj = core_obj_ref; //this tells it where the core_obj is

            }
            else
            {
                Debug.LogWarning($"No prefab found for type: {slot_array[i]}");
            }

            //INITIALIZE Characters to their Entity_Statistics_SO's
            /*
             * 
             * The SO will need to have an array of enums. The index positions indicates
             * the slot it will go to. Each enum contains character prefab object and then
             * and associated Entity_Statistics_SO.
             * 
             * Will modify this spawner so that it just loops through the enum array
             * instantiating each prefab. Then set the instantiated obj's stats according
             * to the Entity_Stat_SO.
             * 
             * Should probably pass a similar array to squadManager which contains
             * the {instantiated obj ref, its entity stats SO for reference.
             * 
             * But this can wait for later.
             *
             */
            
        }
    }

    public void FillSquadManagerWithSquad()
    {
        //get _squadManager reference
        GameObject squadObj = GameObject.Find("Squad Manager");
        if (squadObj != null)
        {
            _squadManager = squadObj.GetComponent<SquadManager>();
        }
        if (_squadManager == null)
        {
            Debug.LogError("SpawnManager: SquadManager not found or missing component!");
        }
        //set core object in SquadManager
        _squadManager.SetCoreObj(core_obj_ref);

        //set character slots in SquadManager
        _squadManager.SetCharactersInSlots(character_ref_array);
    }

    
    
}
