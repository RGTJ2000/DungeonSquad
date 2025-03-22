using UnityEngine;

//[CreateAssetMenu(fileName = "New Skill_SO", menuName = "Skills/Skill_SO")]
public abstract class Skill_SO : ScriptableObject
{
    public string skill_type;
    public string skill_name;
    public Targeting_Type skill_targetType;
    //public string skill_targetType; //other, group, self, area
    public float cooldown; //set to 0 if no cooldown
    public Sprite skill_icon;

    public abstract void Use(GameObject user,  GameObject target);
}
