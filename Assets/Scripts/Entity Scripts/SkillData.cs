using UnityEngine;

[System.Serializable]
public struct SkillData
{
    public string SkillType; // Element 1: Skill Type
    public int SkillID;      // Element 2: Skill ID
    public string SkillName; // Name of skill
    public string SkillTargetingTag;
    public Sprite SkillIcon;
    public Spell_SO SpellSO;

    // Optional: Add a constructor for convenience
    public SkillData(string skillType, int skillID, string skillName, string skillTargetingTag, Sprite skillIcon, Spell_SO spell)
    {
        SkillType = skillType;
        SkillID = skillID;
        SkillName = skillName;
        SkillTargetingTag = skillTargetingTag;
        SkillIcon = skillIcon;
        SpellSO = spell;
        
    }
}
