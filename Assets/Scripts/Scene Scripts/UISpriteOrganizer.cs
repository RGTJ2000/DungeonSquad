using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NamedSprite
{
    public string name; // Name associated with the sprite
    public Sprite sprite; // The sprite itself
}

[System.Serializable]
public class SpriteCategory
{
    public string categoryName; // The name of the category (e.g., "melee")
    public NamedSprite[] sprites;    // Array of sprites for this category
    
}


public class UISpriteOrganizer : MonoBehaviour
{
    // Singleton instance
    public static UISpriteOrganizer Instance { get; private set; }

    // List to hold categories and their sprites (visible in the Inspector)
    public List<SpriteCategory> spriteCategoriesList;

    // Dictionary to hold sprite arrays categorized by strings
    private Dictionary<string, NamedSprite[]> spriteCategories;

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Debug.LogWarning("Duplicate SpriteOrganizer instance detected. Destroying the new one.");
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        // Initialize the dictionary
        spriteCategories = new Dictionary<string, NamedSprite[]>();

        // Populate the dictionary from the list
        foreach (var category in spriteCategoriesList)
        {
            if (!spriteCategories.ContainsKey(category.categoryName))
            {
                spriteCategories.Add(category.categoryName, category.sprites);
            }
            else
            {
                Debug.LogWarning("Duplicate category name: " + category.categoryName);
            }
        }
    }

    // Method to get a sprite by category and index
    public Sprite GetSprite(string category, int index)
    {
        if (spriteCategories.ContainsKey(category))
        {
            NamedSprite[] namedSprites = spriteCategories[category];

            if (index >= 0 && index < namedSprites.Length)
            {
                return namedSprites[index].sprite;
            }
            else
            {
                Debug.LogWarning("Index out of range for category: " + category);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Category not found: " + category);
            return null;
        }
    }

    public string GetSpriteName(string category, int index)
    {
        if (spriteCategories.ContainsKey(category))
        {
            NamedSprite[] namedSprites = spriteCategories[category];

            if (index >= 0 && index < namedSprites.Length)
            {
                return namedSprites[index].name;
            }
            else
            {
                Debug.LogWarning("Index out of range for category: " + category);
                return null;
            }
        }
        else
        {
            Debug.LogWarning("Category not found: " + category);
            return null;
        }
    }
}
