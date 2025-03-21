using UnityEngine;
using System.Collections.Generic;

public class WeaponDatabase : MonoBehaviour
{
    // Singleton instance
    public static WeaponDatabase Instance { get; private set; }

    // Dictionary to store weapons by name
    private Dictionary<string, Weapon_SO> weaponDictionary;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            InitializeDatabase();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    // Initialize the database by loading all Weapon_SO assets
    private void InitializeDatabase()
    {
        weaponDictionary = new Dictionary<string, Weapon_SO>();

        // Load all Weapon_SO assets from the "Resources/Weapons" folder
        Weapon_SO[] weapons = Resources.LoadAll<Weapon_SO>("Weapons");

        // Add each weapon to the dictionary
        foreach (Weapon_SO weapon in weapons)
        {
            if (!weaponDictionary.ContainsKey(weapon.weaponName))
            {
                weaponDictionary.Add(weapon.weaponName, weapon);
            }
            else
            {
                Debug.LogWarning($"Duplicate weapon name found: {weapon.weaponName}. Skipping.");
            }
        }

        Debug.Log($"Weapon database initialized with {weaponDictionary.Count} weapons.");
    }

    // Get a weapon by name
    public Weapon_SO GetWeaponByName(string weaponName)
    {
        if (weaponDictionary.TryGetValue(weaponName, out Weapon_SO weapon))
        {
            return weapon;
        }
        else
        {
            Debug.LogWarning($"Weapon with name '{weaponName}' not found in database.");
            return null;
        }
    }

    // Get all weapons in the database
    public List<Weapon_SO> GetAllWeapons()
    {
        return new List<Weapon_SO>(weaponDictionary.Values);
    }
}