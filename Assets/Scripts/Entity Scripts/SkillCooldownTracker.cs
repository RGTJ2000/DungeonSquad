using System.Collections.Generic;
using UnityEngine;

public class SkillCooldownTracker : MonoBehaviour
{
    [System.Serializable]
    public struct SkillCooldownPair
    {
        public Skill_SO skill;  // Key: The skill
        public float cooldown;  // Value: Cooldown duration
    }

    [SerializeField]
    private List<SkillCooldownPair> cooldownPairs = new List<SkillCooldownPair>();

    private Dictionary<Skill_SO, float> cooldowns = new Dictionary<Skill_SO, float>();

    private void Awake()
    {
        SyncDictionaryFromList(); // Initialize the dictionary from the list
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) // Only sync in the Editor
        {
            SyncDictionaryFromList();
        }
    }

    private void Update()
    {
        List<Skill_SO> keys = new List<Skill_SO>(cooldowns.Keys);

        foreach (var skill in keys)
        {
            cooldowns[skill] -= Time.deltaTime;

            if (cooldowns[skill] <= 0f)
            {
                cooldowns.Remove(skill);
                SyncListFromDictionary(); // Sync only when a cooldown expires
            }
        }
#if UNITY_EDITOR
        // Sync the list every frame for testing purposes (Editor only)
        SyncListFromDictionary();
#endif
    }

    public void StartCooldown(Skill_SO skill)
    {
        if (skill == null)
        {
            Debug.LogWarning("Attempted to start cooldown for a null skill.");
            return;
        }

        float cooldownDuration = skill.cooldown;

        if (!cooldowns.ContainsKey(skill))
        {
            cooldowns.Add(skill, cooldownDuration);
        }
        else
        {
            //cooldowns[skill] = cooldownDuration;
        }

        SyncListFromDictionary(); // Sync after starting a new cooldown
    }

    public bool IsSkillOnCooldown(Skill_SO skill)
    {
        return cooldowns.ContainsKey(skill);
    }

    public float GetRemainingCooldown(Skill_SO skill)
    {
        if (cooldowns.ContainsKey(skill))
        {
            return cooldowns[skill];
        }
        else
        {
            return 0f;

        }
    }

    private void SyncListFromDictionary()
    {
        cooldownPairs.Clear();

        foreach (var kvp in cooldowns)
        {
            cooldownPairs.Add(new SkillCooldownPair { skill = kvp.Key, cooldown = kvp.Value });
        }
    }

    private void SyncDictionaryFromList()
    {
        cooldowns.Clear();

        for (int i = cooldownPairs.Count - 1; i >= 0; i--)
        {
            var pair = cooldownPairs[i];

            if (pair.skill == null)
            {
                Debug.LogWarning("Null skill found in cooldownPairs, removing.");
                cooldownPairs.RemoveAt(i); // Remove null entries
                continue;
            }

            if (!cooldowns.ContainsKey(pair.skill))
            {
                cooldowns.Add(pair.skill, pair.cooldown);
            }
            else
            {
                Debug.LogWarning($"Duplicate skill '{pair.skill.name}' found in cooldownPairs, skipping.");
            }
        }
    }
}