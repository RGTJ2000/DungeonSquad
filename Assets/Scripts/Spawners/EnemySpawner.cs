using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EntityLoadout_SO[] enemy_loudouts_array;
   
    [SerializeField] int number_of_enemies = 5;

    private int max_attempts = 100; //number of attempts to spawn each enemy


    //Level boundaries, to be read from the level creator object/script after level is created
    [SerializeField] private float boundmax_x = 20f;
    [SerializeField] private float boundmax_z = 20f;

    public void SpawnEnemies()
    {

        for (int i = 0; i < number_of_enemies; i++)
        {
            EntityLoadout_SO enemyLoadout = GetRandomEnemyPrefab();
            GameObject enemyPrefab = enemyLoadout.entity_prefab;

            EntityStats _prefaSbtats = enemyPrefab.GetComponent<EntityStats>();

            bool spawnSuccssful = false;

            for (int j = 0; j < max_attempts; j++)
            {

                Vector3 random_position = new Vector3(Random.Range(-boundmax_x, boundmax_x), 0, Random.Range(-boundmax_z, boundmax_z));

                if (!Physics.CheckSphere(random_position + Vector3.up, _prefaSbtats.entity_radius))
                {
                    
                    
                    GameObject newEnemy = Instantiate(enemyPrefab, random_position+Vector3.up, Quaternion.identity);
                    spawnSuccssful = true;

                    EntityStats newEnemyStats = newEnemy.GetComponent<EntityStats>();
                    Inventory newEnemyInventory = newEnemy.GetComponent<Inventory>();

                    LoadStatsAndInventory(enemyLoadout, newEnemyStats, newEnemyInventory);

                    newEnemyStats.UpdateAdjustedStats();
                    break;
                }
            }

            if (!spawnSuccssful) 
            {
                Debug.Log("Spawn #" + i+ " failed");
            }

        }


    }

    private EntityLoadout_SO GetRandomEnemyPrefab()
    {
        return enemy_loudouts_array[Random.Range(0, enemy_loudouts_array.Length)];
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
        stats.characterName = loadout.characterName;
        stats.characterPortrait = loadout.characterPortrait;

        stats.visible_distance = loadout.visible_distance;
        stats.health_max = loadout.health_max;
        stats.health_current = loadout.health_max;
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
        else
        {
            stats.equipped_meleeWeapon = null;
        }

        if (loadout.equipped_rangedWeapon?.baseItem is Ranged_Weapon_SO rangedSO)
        {
            stats.equipped_rangedWeapon = new RuntimeItem(rangedSO);
        }
        else
        {
            stats.equipped_rangedWeapon = null;
        }

        if (loadout.equipped_missile?.baseItem is Missile_SO missileSO)
        {
            stats.equipped_missile = new RuntimeItem(missileSO);
        }
        else
        {
            stats.equipped_missile = null;
        }

        if (loadout.equipped_ring?.baseItem is Ring_SO ringSO)
        {
            //Debug.Log("setting ring to ringSO");
            stats.equipped_ring = new RuntimeItem(ringSO);
        }
        else
        {
            //Debug.Log("Setting ring to null");
            stats.equipped_ring = null;
        }

        if (loadout.equipped_helm?.baseItem is Helm_SO helmSO)
        {
            stats.equipped_helm = new RuntimeItem(helmSO);
        }
        else
        {
            stats.equipped_helm = null;
        }

        if (loadout.equipped_amulet?.baseItem is Amulet_SO amuletSO)
        {
            stats.equipped_amulet = new RuntimeItem(amuletSO);
        }
        else
        {
            stats.equipped_amulet = null;
        }

        if (loadout.equipped_armor?.baseItem is Armor_SO armorSO)
        {
            stats.equipped_armor = new RuntimeItem(armorSO);
        }
        else
        {
            Debug.Log("Enemy stats armor set to null.");
            stats.equipped_armor = null;
        

        }

        if (loadout.equipped_shield?.baseItem is Shield_SO shieldSO)
        {
            stats.equipped_shield = new RuntimeItem(shieldSO);
        }
        else
        {
            stats.equipped_shield = null;
        }

        if (loadout.equipped_boots?.baseItem is Boots_SO bootsSO)
        {
            stats.equipped_boots = new RuntimeItem(bootsSO);
        }
        else
        {
            stats.equipped_boots = null;
        }




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

        //Debug.Log($"Loaded stats and inventory for character: {loadout.character_name}");
    }




}
