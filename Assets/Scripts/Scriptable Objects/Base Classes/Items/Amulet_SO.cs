using UnityEngine;

[CreateAssetMenu(fileName = "Amulet_SO", menuName = "Item/Amulet_SO")]
public class Amulet_SO : Item_SO
{
    public float defensePhysical_modifier;

    public float defenseConfusion_modifier;
    public float defenseFear_modifier;

    public float defenseFire_modifier;
    public float defenseFrost_modifier;

    public float defensePoison_modifier;
    public float defenseSleep_modifier;
}
