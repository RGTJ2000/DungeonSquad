using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class SquadSpawner : MonoBehaviour
{
    [SerializeField] private GameObject core_prefab;

    [SerializeField] private GameObject fighter_prefab;
    [SerializeField] private EntityLoadout_SO fighter_loadout;

    [SerializeField] private GameObject cleric_prefab;
    [SerializeField] private EntityLoadout_SO cleric_loadout;

    [SerializeField] private GameObject wizard_prefab;
    [SerializeField] private EntityLoadout_SO wizard_loadout;

    [SerializeField] private GameObject ranger_prefab;
    [SerializeField] private EntityLoadout_SO ranger_loadout;


    private GameObject core_obj_ref;
    private GameObject[] character_ref_array;
    private Vector3 spawnPoint;

    //Get these values from a future SQUAD_CONFIGDATA_SO...maybe?
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
            EntityLoadout_SO characterLoadout;

            switch (slot_array[i])
            {
                case Character_Type.fighter:
                    prefabToInstantiate = fighter_prefab;
                    characterLoadout = fighter_loadout;
                    break;
                case Character_Type.cleric:
                    prefabToInstantiate = cleric_prefab;
                    characterLoadout = cleric_loadout;
                    break;
                case Character_Type.wizard:
                    prefabToInstantiate = wizard_prefab;
                    characterLoadout = wizard_loadout;  
                    break;
                case Character_Type.ranger:
                    prefabToInstantiate = ranger_prefab;
                    characterLoadout = ranger_loadout;
                    break;
                default:
                    prefabToInstantiate = null;
                    characterLoadout = null;
                    break;
            }

            // Instantiate the prefab if it's found
            if (prefabToInstantiate != null)
            {
                character_ref_array[i] = Instantiate(prefabToInstantiate, spawnPoint + Vector3.right * 2f, Quaternion.identity);

                Ch_Behavior _chBehavior = character_ref_array[i].GetComponent<Ch_Behavior>();
                _chBehavior.slot_num = i;  //this tells the character what slot to follow
                _chBehavior.core_obj = core_obj_ref; //this tells it where the core_obj is

                EntityStats _chStats = character_ref_array[i].GetComponent<EntityStats>();
                Inventory _chInventory = character_ref_array[i].GetComponent<Inventory>();

                if (characterLoadout != null)
                {
                    LoadStatsAndInventory(characterLoadout, _chStats, _chInventory);
                }
                

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


    private void LoadStatsAndInventory(EntityLoadout_SO loadout, EntityStats stats, Inventory inventory)
    {
        if (loadout == null || stats == null || inventory == null)
        {
            Debug.LogWarning("Loadout, stats, or inventory for is null!");
            return;
        }

        // --- BASIC STATS ---
        stats.character_ID = loadout.character_ID;
        stats.visible_distance = loadout.visible_distance;
        stats.health_max = loadout.health_max;
        stats.walking_speed = loadout.walking_speed;
        stats.running_speed = loadout.running_speed;
        stats.entity_radius = loadout.entity_radius;

        stats.strength = loadout.strength;
        stats.dexterity = loadout.dexterity;
        stats.intelligence = loadout.intelligence;
        stats.will = loadout.will;
        stats.soul = loadout.soul;

        stats.melee_attackRating = loadout.melee_attackRating;
        stats.melee_defenseRating = loadout.melee_defenseRating;
        stats.ranged_attackRating = loadout.ranged_attackRating;
        stats.ranged_defenseRating = loadout.ranged_defenseRating;
        stats.magic_attackRating = loadout.magic_attackRating;

        stats.confusion_defenseRating = loadout.confusion_defenseRating;
        stats.fear_defenseRating = loadout.fear_defenseRating;
        stats.fire_defenseRating = loadout.fire_defenseRating;
        stats.frost_defenseRating = loadout.frost_defenseRating;
        stats.poison_defenseRating = loadout.poison_defenseRating;
        stats.sleep_defenseRating = loadout.sleep_defenseRating;

        // --- EQUIPPED ITEMS ---

        if (loadout.equipped_meleeWeapon?.baseItem is Melee_Weapon_SO meleeSO)
        {
            stats.equipped_meleeWeapon = new RuntimeItem(meleeSO);
        }

        if (loadout.equipped_rangedWeapon?.baseItem is Ranged_Weapon_SO rangedSO)
        {
            stats.equipped_rangedWeapon = new RuntimeItem(rangedSO);
        }

        if (loadout.equipped_missile?.baseItem is Missile_SO missileSO)
        {
            stats.equipped_missile = new RuntimeItem(missileSO);
        }
        /*
        stats.equipped_meleeWeapon = loadout.equipped_meleeWeapon?.baseItem as Melee_Weapon_SO;
        stats.equipped_rangedWeapon = loadout.equipped_rangedWeapon?.baseItem as Ranged_Weapon_SO;
        stats.equipped_missile = loadout.equipped_missile?.baseItem as Missile_SO;
        */

        // --- STATUS LIMITS / DISSIPATION ---
        stats.confusion_AL = loadout.confusion_AL;
        stats.fear_AL = loadout.fear_AL;
        stats.fire_AL = loadout.fire_AL;
        stats.frost_AL = loadout.frost_AL;
        stats.poison_AL = loadout.poison_AL;
        stats.sleep_AL = loadout.sleep_AL;

        stats.confusion_dissipationRate = loadout.confusion_dissipationRate;
        stats.fear_dissipationRate = loadout.fear_dissipationRate;
        stats.fire_dissipationRate = loadout.fire_dissipationRate;
        stats.frost_dissipationRate = loadout.frost_dissipationRate;
        stats.poison_dissipationRate = loadout.poison_dissipationRate;
        stats.sleep_dissipationRate = loadout.sleep_dissipationRate;

        stats.confusion_damageMultiplier = loadout.confusion_damageMultiplier;
        stats.fear_damageMultiplier = loadout.fear_damageMultiplier;
        stats.fire_damageMultiplier = loadout.fire_damageMultiplier;
        stats.frost_damageMultiplier = loadout.frost_damageMultiplier;
        stats.poison_damageMultiplier = loadout.poison_damageMultiplier;
        stats.sleep_damageMultiplier = loadout.sleep_damageMultiplier;

        // --- SKILLS ---
        stats.skill_slot = loadout.skill_slot;
        stats.maxSkillSlots = loadout.maxSkillSlots;
        stats.active_skillSlot = loadout.active_skillSlot;

        if (stats.skill_slot.Length > 0)
            stats.selected_skill = stats.skill_slot[0]; // default selection

        // --- INVENTORY ---
        inventory.Clear(); // assuming your Inventory class has a Clear method

        foreach (var itemData in loadout.inventoryItems)
        {
            if (itemData.baseItem != null)
            {
                RuntimeItem runtimeItem = new RuntimeItem(itemData.baseItem);
                runtimeItem.stackCount = itemData.stackCount;
                runtimeItem.attachedCharms.AddRange(itemData.attachedCharms); // Copy over charms
                inventory.AddItem(runtimeItem); // assuming AddItem() exists
            }
        }

        Debug.Log($"Loaded stats and inventory for character: {loadout.character_name}");
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
