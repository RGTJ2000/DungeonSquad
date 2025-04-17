#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Weapon_SO_old))]
public class WeaponSOEditor_old : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the target Weapon_SO
        Weapon_SO_old weapon = (Weapon_SO_old)target;

        // Draw UNIVERSAL STATS (always shown)
        EditorGUILayout.LabelField("Universal Stats", EditorStyles.boldLabel);
        weapon.weaponName = EditorGUILayout.TextField("Weapon Name", weapon.weaponName);
        weapon.weaponType = (weaponType)EditorGUILayout.EnumPopup("Weapon Type", weapon.weaponType);
        weapon.hitAudio_variationID = EditorGUILayout.TextField("Hit Audio Var ID", weapon.hitAudio_variationID);
        weapon.missAudio_variationID = EditorGUILayout.TextField("Miss Audio Var ID", weapon.missAudio_variationID);

        // Draw COMMON STATS (only for Melee and Ranged)
        if (weapon.weaponType == weaponType.melee || weapon.weaponType == weaponType.ranged)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Common Stats (Melee and Ranged)", EditorStyles.boldLabel);
            weapon.attackCooldown = EditorGUILayout.FloatField("Attack Cooldown", weapon.attackCooldown);

            // Draw CRIT CHANCE STATS
            EditorGUILayout.LabelField("Crit Chance Stats", EditorStyles.boldLabel);
            weapon.critChance_base = EditorGUILayout.Slider("Crit Chance Base", weapon.critChance_base, 0, 1);
            weapon.critChance_weight_str = EditorGUILayout.Slider("Crit Chance Weight (Str)", weapon.critChance_weight_str, 0, 1);
            weapon.critChance_weight_dex = EditorGUILayout.Slider("Crit Chance Weight (Dex)", weapon.critChance_weight_dex, 0, 1);
            weapon.critChance_weight_int = EditorGUILayout.Slider("Crit Chance Weight (Int)", weapon.critChance_weight_int, 0, 1);
            weapon.critChance_weight_will = EditorGUILayout.Slider("Crit Chance Weight (Will)", weapon.critChance_weight_will, 0, 1);
            weapon.critChance_weight_soul = EditorGUILayout.Slider("Crit Chance Weight (Soul)", weapon.critChance_weight_soul, 0, 1);
        }

        // Draw MELEE-specific stats
        if (weapon.weaponType == weaponType.melee)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Melee Stats", EditorStyles.boldLabel);
            weapon.melee_reach = EditorGUILayout.FloatField("Melee Reach", weapon.melee_reach);
            weapon.melee_damageBase = EditorGUILayout.FloatField("Melee Damage Base", weapon.melee_damageBase);
            weapon.melee_damageRange = EditorGUILayout.FloatField("Melee Damage Range", weapon.melee_damageRange);
        }

        // Draw RANGED-specific stats
        else if (weapon.weaponType == weaponType.ranged)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Ranged Stats", EditorStyles.boldLabel);
            weapon.launch_impulse = EditorGUILayout.FloatField("Launch Impulse", weapon.launch_impulse);
            weapon.accuracy_factor = EditorGUILayout.FloatField("Accuracy Factor", weapon.accuracy_factor);
        }

        // Draw MISSILE-specific stats
        else if (weapon.weaponType == weaponType.missile)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Missile Stats", EditorStyles.boldLabel);
            weapon.missile_damageBase = EditorGUILayout.FloatField("Missile Damage Base", weapon.missile_damageBase);
            weapon.missile_damageRange = EditorGUILayout.FloatField("Missile Damage Range", weapon.missile_damageRange);
            weapon.missile_weight = EditorGUILayout.FloatField("Missile Weight", weapon.missile_weight);
            weapon.missile_prefab = EditorGUILayout.ObjectField("Missile Prefab", weapon.missile_prefab, typeof(GameObject), false) as GameObject;
        }

        // Mark the object as dirty if any fields were modified
        if (GUI.changed)
        {
            EditorUtility.SetDirty(weapon);
        }
    }
}
#endif