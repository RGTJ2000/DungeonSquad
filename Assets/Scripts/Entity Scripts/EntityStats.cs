using UnityEngine;

public enum StatCategory
{
    strength,
    dexterity,
    intelligence,
    will,
    soul
}

public class EntityStats : MonoBehaviour
{
    public int character_ID = 0;
    public string characterName;
    public Sprite characterPortrait;
    public float visible_distance = 50f;
    public float health_max = 100f;
    public float health_current;

    public float walking_speed = 5.0f;
    public float running_speed = 10.0f;

    public float entity_radius = 0.5f;
    public float entity_reach = 3.0f;

    public float strength = 50f;
    public float dexterity = 50f;
    public float intelligence = 50f;
    public float will = 50f;
    public float soul = 50f;

    public float str_adjusted;
    public float dex_adjusted;
    public float int_adjusted;
    public float will_adjusted;
    public float soul_adjusted;

    public float melee_attackRating = 15f;
    public float melee_defenseRating = 10f;

    public float dodgeChance;
    public float blockChance;
    public float parryChance;

    public float ranged_attackRating = 15f;
    public float ranged_defenseRating = 10f;
    public float degrees_of_accuracy = 0f;

    public float magic_attackRating = 15f;
    //public float magic_defenseRating = 10f;
    public float confusion_defenseRating = 10f;
    public float fear_defenseRating = 10f;
    public float fire_defenseRating = 10f;
    public float frost_defenseRating = 10f;
    public float poison_defenseRating = 10f;
    public float sleep_defenseRating = 10f;


    public RuntimeItem equipped_meleeWeapon = null;
    public RuntimeItem equipped_rangedWeapon = null;
    public RuntimeItem equipped_missile = null;

    public RuntimeItem equipped_ring = null;
    public RuntimeItem equipped_helm = null;
    public RuntimeItem equipped_amulet = null;
    public RuntimeItem equipped_armor = null;
    public RuntimeItem equipped_shield = null;
    public RuntimeItem equipped_boots = null;


    public string active_targetingTag = "Enemy";

    public int maxSkillSlots = 5;

    //public SkillData[] skill_slot;
    [SerializeField] public Skill_SO[] skill_slot;
    public int active_skillSlot = 0;

    public Skill_SO selected_skill;

    public float confusion_AL = 20;
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

    private void Awake()
    {


        str_adjusted = strength;
        dex_adjusted = dexterity;
        int_adjusted = intelligence;
        will_adjusted = will;
        soul_adjusted = soul;
    }

    private void Start()
    {
        if (skill_slot.Length > 0)
        {
            selected_skill = skill_slot[0]; //set active skill to first slot by default 
        }



    }

    public void UpdateAdjustedStats()
    {

        str_adjusted = AdjustStatToEquipment(strength, StatCategory.strength);
        dex_adjusted = AdjustStatToEquipment(dexterity, StatCategory.dexterity);
        int_adjusted = AdjustStatToEquipment(intelligence, StatCategory.intelligence);
        will_adjusted = AdjustStatToEquipment(will, StatCategory.will);
        soul_adjusted = AdjustStatToEquipment(soul, StatCategory.soul);

        // Debug.Log($"{gameObject.name} AdjStats: str:{str_adjusted}, dex:{dex_adjusted}, int:{int_adjusted}, will:{will_adjusted}, soul:{soul_adjusted}");
        UpdateAttackDefenseRatings();
    }

    private float AdjustStatToEquipment(float stat, StatCategory category)
    {
        float adjustedStat;
        float totalAdjustment = 0f;

        if (equipped_ring != null)
        {
            totalAdjustment += ReturnAdjustmentToCategory(equipped_ring, category);
        }

        if (equipped_helm != null)
        {
            totalAdjustment += ReturnAdjustmentToCategory(equipped_helm, category);
        }

        if (equipped_amulet != null)
        {
            totalAdjustment += ReturnAdjustmentToCategory(equipped_amulet, category);

        }

        if (equipped_meleeWeapon != null)
        {
            totalAdjustment += ReturnAdjustmentToCategory(equipped_meleeWeapon, category);

        }

        if (equipped_armor != null)
        {
            totalAdjustment += ReturnAdjustmentToCategory(equipped_armor, category);
        }

        if (equipped_rangedWeapon != null)
        {
            totalAdjustment += ReturnAdjustmentToCategory(equipped_rangedWeapon, category);
        }

        if (equipped_shield != null)
        {
            totalAdjustment += ReturnAdjustmentToCategory(equipped_shield, category);
        }

        if (equipped_boots != null)
        {
            totalAdjustment += ReturnAdjustmentToCategory(equipped_boots, category);
        }


        adjustedStat = stat + totalAdjustment;

        return adjustedStat;
    }

    private float ReturnAdjustmentToCategory(RuntimeItem item, StatCategory category)
    {
        float adjustment = 0f;

        switch (category)
        {
            case StatCategory.strength:
                adjustment = CalculateAdjustment(strength, item.strModifier);
                break;
            case StatCategory.dexterity:
                adjustment = CalculateAdjustment(dexterity, item.dexModifier);
                break;
            case StatCategory.intelligence:
                adjustment = CalculateAdjustment(intelligence, item.intModifier);
                break;
            case StatCategory.will:
                adjustment = CalculateAdjustment(will, item.willModifier);
                break;
            case StatCategory.soul:
                adjustment = CalculateAdjustment(soul, item.soulModifier);
                break;
            default:
                adjustment = 0f;
                break;

        }

        return adjustment;
    }

    private float CalculateAdjustment(float stat, StatAdjustment statadjustment)
    {
        float value;

        switch (statadjustment.operatorType)
        {
            case (OperatorType.percent):
                value = stat * statadjustment.amount;
                break;
            case (OperatorType.additive):
                value = statadjustment.amount;
                break;
            default:
                value = 0f;
                break;
        }

        return value;
    }


    public void UpdateAttackDefenseRatings()
    {
        //calculate melee_DR and dodge-block-parry chances
        float DR_str = str_adjusted;
        if (equipped_shield != null)
        {
            DR_str += str_adjusted * equipped_shield.Shield.defense_strModifier;
        }

        float DR_dex = dex_adjusted;
        if (equipped_meleeWeapon != null)
        {
            DR_dex += dex_adjusted * equipped_meleeWeapon.MeleeWeapon.defense_dexModifier;
        }

        float DR_totalSum = DR_str + DR_dex;

        melee_defenseRating = DR_totalSum / 2;

        float dexSum = dex_adjusted + DR_dex;

        blockChance = Mathf.Clamp(DR_str / DR_totalSum, 0, 1);
        dodgeChance = Mathf.Clamp((1 - blockChance) * (dex_adjusted / dexSum), 0, 1);
        parryChance = Mathf.Clamp(1 - (blockChance + dodgeChance), 0, 1);


        //calculate melee_AR
        float AR_str = str_adjusted;
        float AR_dex = dex_adjusted;
        if (equipped_meleeWeapon != null)
        {
            AR_str += str_adjusted * equipped_meleeWeapon.MeleeWeapon.attack_strModifier;
            AR_dex += dex_adjusted * equipped_meleeWeapon.MeleeWeapon.attack_dexModifier;
        }

        melee_attackRating = (AR_str + AR_dex) / 2;



        //calculate ranged_DR
        float ranged_DR_dex = dex_adjusted;
        if (equipped_shield != null)
        {
            ranged_DR_dex += dex_adjusted * equipped_shield.Shield.defense_dexModifier;
        }

        ranged_defenseRating = ranged_DR_dex;

        //calculate ranged_AR
        float ranged_AR_dex = dex_adjusted;
        if (equipped_rangedWeapon != null)
        {
            ranged_AR_dex += dex_adjusted * equipped_rangedWeapon.RangedWeapon.attack_dexModifier;
        }

        if (equipped_missile != null)
        {
            ranged_AR_dex += dex_adjusted * equipped_missile.Missile.attack_dexModifier;
        }

        ranged_attackRating = (ranged_AR_dex + int_adjusted) / 2;

        degrees_of_accuracy = 1f / ((0.0015f * ranged_attackRating) + (1f / 180f));

        //Debug.Log($"{gameObject.name}: melee_DR {melee_defenseRating}, blockChance={blockChance}, dodgeChance={dodgeChance}, parryChance={parryChance}, melee_AR: {melee_attackRating}, ranged_AR={ranged_attackRating}");

        //calculate magic_AR
        float AR_int = int_adjusted;
        float AR_will = will_adjusted;
        if (equipped_meleeWeapon != null)
        {
            AR_int += int_adjusted * equipped_meleeWeapon.MeleeWeapon.attack_intModifier;
            AR_will += will_adjusted * equipped_meleeWeapon.MeleeWeapon.attack_willModifer;
        }

        magic_attackRating = (AR_int + AR_will) / 2;

    }

    public RuntimeItem GetEquippedByCategory(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.ring:
                return equipped_ring;
            case ItemCategory.helm:
                return equipped_helm;
            case ItemCategory.amulet:
                return equipped_amulet;

            case ItemCategory.melee_weapon:
                return equipped_meleeWeapon;
            case ItemCategory.armor:
                return equipped_armor;
            case ItemCategory.ranged_weapon:
                return equipped_rangedWeapon;

            case ItemCategory.shield:
                return equipped_shield;
            case ItemCategory.boots:
                return equipped_boots;
            case ItemCategory.missile:
                return equipped_missile;

            default:
                return null;

        }
    }
}


