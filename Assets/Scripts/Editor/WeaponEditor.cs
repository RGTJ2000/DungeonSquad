using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Weapon_SO), true)]
public class WeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Weapon_SO weapon = (Weapon_SO)target;

        // Draw shared Item_SO fields
        weapon.item_name = EditorGUILayout.TextField("Item Name", weapon.item_name);
        weapon.description = EditorGUILayout.TextArea(weapon.description);
        weapon.category = (ItemCategory)EditorGUILayout.EnumPopup("Category", weapon.category);
        weapon.item_icon = (Sprite)EditorGUILayout.ObjectField("Item Icon", weapon.item_icon, typeof(Sprite), false);
        weapon.item_prefab = (GameObject)EditorGUILayout.ObjectField("Item Prefab", weapon.item_prefab, typeof(GameObject), false);
        weapon.pickupAudio_ID = EditorGUILayout.TextField("pickupAudio_ID", weapon.pickupAudio_ID);
        weapon.dropAudio_ID = EditorGUILayout.TextField("dropAudio_ID", weapon.dropAudio_ID);

        weapon.isStackable = EditorGUILayout.Toggle("Is Stackable", weapon.isStackable);
        if (weapon.isStackable)
            weapon.maxStack = EditorGUILayout.IntField("Max Stack", weapon.maxStack);

        EditorGUILayout.Space();
        weapon.attackCooldown = EditorGUILayout.FloatField("Attack Cooldown", weapon.attackCooldown);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Crit Settings", EditorStyles.boldLabel);
        weapon.critChance_base = EditorGUILayout.Slider("Crit Chance Base", weapon.critChance_base, 0f, 1f);
        weapon.critChance_weight_str = EditorGUILayout.Slider("Crit Chance Weight (STR)", weapon.critChance_weight_str, 0f, 1f);
        weapon.critChance_weight_dex = EditorGUILayout.Slider("Crit Chance Weight (DEX)", weapon.critChance_weight_dex, 0f, 1f);
        weapon.critChance_weight_int = EditorGUILayout.Slider("Crit Chance Weight (INT)", weapon.critChance_weight_int, 0f, 1f);
        weapon.critChance_weight_will = EditorGUILayout.Slider("Crit Chance Weight (WILL)", weapon.critChance_weight_will, 0f, 1f);
        weapon.critChance_weight_soul = EditorGUILayout.Slider("Crit Chance Weight (SOUL)", weapon.critChance_weight_soul, 0f, 1f);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Specific Weapon Fields", EditorStyles.boldLabel);
        DrawPropertiesExcluding(serializedObject,
            "m_Script",
            "item_name",
            "description",
            "category",
            "item_icon",
            "item_prefab",
            "pickupAudio_ID",
            "dropAudio_ID",
            "isStackable",
            "maxStack",
            "attackCooldown",
            "critChance_base",
            "critChance_weight_str",
            "critChance_weight_dex",
            "critChance_weight_int",
            "critChance_weight_will",
            "critChance_weight_soul"
        );
        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(weapon);
        }
    }
}
