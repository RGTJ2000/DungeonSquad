using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityStatistics_SO", menuName = "Entity Data/EntityLoadout_SO")]
public class EntityLoadout_SO : ScriptableObject
{
    public int character_ID = 0;
    public string characterName;
    public Sprite characterPortrait;

    public float visible_distance = 50f;
    public float health_max = 100f;

    public float walking_speed = 5.0f;
    public float running_speed = 10.0f;

    public float entity_radius = 0.5f;

    public float strength = 50f;
    public float dexterity = 50f;
    public float intelligence = 50f;
    public float will = 50f;
    public float soul = 50f;

    public float melee_attackRating = 15f;
    public float melee_defenseRating = 10f;

    public float ranged_attackRating = 15f;
    public float ranged_defenseRating = 10f;

    public float magic_attackRating = 15f;
    //public float magic_defenseRating = 10f;
    public float confusion_defenseRating = 10f;
    public float fear_defenseRating = 10f;
    public float fire_defenseRating = 10f;
    public float frost_defenseRating = 10f;
    public float poison_defenseRating = 10f;
    public float sleep_defenseRating = 10f;


    public StartingItemData equipped_meleeWeapon;
    public StartingItemData equipped_rangedWeapon;
    public StartingItemData equipped_missile;

    public StartingItemData equipped_ring;
    public StartingItemData equipped_helm;
    public StartingItemData equipped_amulet;
    public StartingItemData equipped_armor;
    public StartingItemData equipped_shield;
    public StartingItemData equipped_boots;



    public int maxSkillSlots = 5;

    public Skill_SO[] skill_slot;
    public int active_skillSlot = 0;

    

    public float confusion_AL = 20;  //activaton limit
    public float fear_AL = 20;
    public float fire_AL = 20;
    public float frost_AL = 20;
    public float poison_AL = 20;
    public float sleep_AL = 20;

    public float confusion_dissipationRate = 1f;
    public float fear_dissipationRate = 1f;
    public float fire_dissipationRate = 2f;
    public float frost_dissipationRate = 1f;
    public float poison_dissipationRate = 1f;
    public float sleep_dissipationRate = 1f;

    public float confusion_damageMultiplier = 1f;
    public float fear_damageMultiplier = 1f;
    public float fire_damageMultiplier = 2f;
    public float frost_damageMultiplier = 1f;
    public float poison_damageMultiplier = 1f;
    public float sleep_damageMultiplier = 1f;

    //Inventory
    public List<StartingItemData> inventoryItems;





}
