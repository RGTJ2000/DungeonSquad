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

    public float melee_critBonusFactor;
    public float ranged_critBonusFactor;

    public float melee_dodgeChance;
    public float melee_blockChance;
    public float melee_parryChance;

    public float ranged_blockChance;
    public float ranged_dodgeChance;

    public float ranged_attackRating = 15f;
    public float ranged_defenseRating = 10f;
    public float degrees_of_accuracy = 0f;

    public float magic_attackRating = 15f;
    public float magic_defenseRating = 10f;

    //****Status Defenses
    public float confusion_defenseRating;
    public float fear_defenseRating;
    public float fire_defenseRating;
    public float frost_defenseRating;
    public float poison_defenseRating;
    public float sleep_defenseRating;

    public float confusion_AL = 20;  //Activation Limit (AL)
    public float fear_AL = 20;
    public float fire_AL = 20;
    public float frost_AL = 20;
    public float poison_AL = 20;
    public float sleep_AL = 20;

    public float confusion_AL_adjusted;
    public float fear_AL_adjusted;
    public float fire_AL_adjusted;
    public float frost_AL_adjusted;
    public float poison_AL_adjusted;
    public float sleep_AL_adjusted;

    public float confusion_dissipationRate = 1f; //Dissipation Factor
    public float fear_dissipationRate = 1f;
    public float fire_dissipationRate = 2f;
    public float frost_dissipationRate = 1f;
    public float poison_dissipationRate = 1f;
    public float sleep_dissipationRate = 1f;

    public float confusion_dissipationRate_adjusted;
    public float fear_dissipationRate_adjusted;
    public float fire_dissipationRate_adjusted;
    public float frost_dissipationRate_adjusted;
    public float poison_dissipationRate_adjusted;
    public float sleep_dissipationRate_adjusted;

    public float magic_damageMultiplier = 1f;
    public float confusion_damageMultiplier = 1f;
    public float fear_damageMultiplier = 1f;
    public float fire_damageMultiplier = 2f;
    public float frost_damageMultiplier = 1f;
    public float poison_damageMultiplier = 1f;
    public float sleep_damageMultiplier = 1f;


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
        //1) adjusted stats
        str_adjusted = AdjustStatToEquipment(strength, StatCategory.strength);
        dex_adjusted = AdjustStatToEquipment(dexterity, StatCategory.dexterity);
        int_adjusted = AdjustStatToEquipment(intelligence, StatCategory.intelligence);
        will_adjusted = AdjustStatToEquipment(will, StatCategory.will);
        soul_adjusted = AdjustStatToEquipment(soul, StatCategory.soul);

        //2) AD DR's
        UpdateAttackDefenseRatings();

        //3 Resists
        UpdateResistStats();

        //4 Update critsModifiers
        UpdateCritChances();
    }

    private void UpdateCritChances()
    {
        

        melee_critBonusFactor =  ReturnStatBonusFactor((str_adjusted+dex_adjusted+will_adjusted)/3) ;
        ranged_critBonusFactor = ReturnStatBonusFactor( (dex_adjusted+int_adjusted+will_adjusted)  / 3) ;
        //Debug.Log("melee_critMod set to "+melee_critModifier+" ranged_critMod set to "+ranged_critModifier+ "-------str_adj="+str_adjusted+ "dex_adj="+dex_adjusted+"will_adj="+will_adjusted);
        

    }

    private void UpdateResistStats()
    {
        //***Resist DRs
        //calculated as the character's base resist modified by their stats

        confusion_defenseRating = CalculateResistDR(int_adjusted, will_adjusted);
        fear_defenseRating =  CalculateResistDR(int_adjusted, soul_adjusted);

        fire_defenseRating = CalculateResistDR(str_adjusted, will_adjusted);
        frost_defenseRating = CalculateResistDR(dex_adjusted, soul_adjusted);

        poison_defenseRating = CalculateResistDR(str_adjusted, soul_adjusted);
        sleep_defenseRating = CalculateResistDR(str_adjusted, int_adjusted);

        //**Resist AL_adjusted
        confusion_AL_adjusted = Mathf.Max(confusion_AL + (confusion_AL * ReturnStatBonusFactor(confusion_defenseRating)), 1f);
        fear_AL_adjusted = Mathf.Max(fear_AL + (fear_AL * ReturnStatBonusFactor(fear_defenseRating)), 1f);
        fire_AL_adjusted = Mathf.Max(fire_AL + (fire_AL * ReturnStatBonusFactor(fire_defenseRating)), 1f);
        frost_AL_adjusted = Mathf.Max(frost_AL + (frost_AL * ReturnStatBonusFactor(frost_defenseRating)), 1f);
        poison_AL_adjusted = Mathf.Max(poison_AL + (poison_AL * ReturnStatBonusFactor(poison_defenseRating)), 1f);
        sleep_AL_adjusted = Mathf.Max(sleep_AL + (sleep_AL * ReturnStatBonusFactor(sleep_defenseRating)), 1f);

        //***Resist DissipationRates adjusted
        confusion_dissipationRate_adjusted = confusion_dissipationRate * (1 + ReturnStatBonusFactor(confusion_defenseRating));
        fear_dissipationRate_adjusted = fear_dissipationRate * (1 + ReturnStatBonusFactor(fear_defenseRating));
        fire_dissipationRate_adjusted = fire_dissipationRate * (1 + ReturnStatBonusFactor(fire_defenseRating));
        frost_dissipationRate_adjusted = frost_dissipationRate * (1 + ReturnStatBonusFactor(frost_defenseRating));
        poison_dissipationRate_adjusted = poison_dissipationRate * (1 + ReturnStatBonusFactor(poison_defenseRating));
        sleep_dissipationRate_adjusted = sleep_dissipationRate * (1 + ReturnStatBonusFactor(sleep_defenseRating));


    }

    private float ReturnStatBonusFactor(float stat)
    {
        return (stat - 50f) / 50f;
    }


    private float CalculateResistDR(float stat1, float stat2)
    {
        return (stat1 + stat2) / 2;

        //add code for amulet resists
    }

    private float CalculateResistAL(float baseResist, float stat1,  float stat2)
    {
        float statAverage = (stat1 + stat2) / 2f;
        float statBonus = (statAverage - 50f)/2f;
        float newValue = baseResist + statBonus;
        Debug.Log("statAve=" + statAverage + " statBonus=" + statBonus);
        return Mathf.Max( newValue, 1f);

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
            DR_str += str_adjusted * equipped_shield.Shield.defense_strBonusFactor;
        }

        float DR_dex = dex_adjusted;
        if (equipped_meleeWeapon != null)
        {
            DR_dex += dex_adjusted * equipped_meleeWeapon.MeleeWeapon.defense_dexBonusFactor;
        }

        float DR_totalSum = DR_str + DR_dex;

        melee_defenseRating = DR_totalSum / 2;


        //MISS CHANCES
        float dexSum = dex_adjusted + DR_dex;


        if (equipped_shield != null)
        {
            //block is the proportion of strength vs dexterity
            melee_blockChance = Mathf.Clamp(DR_str / DR_totalSum, 0, 1);

        }
        else 
        {
            melee_blockChance = 0f;
        }

        ranged_blockChance = melee_blockChance;

        //parry and dodge are dexterity based
        
        if (equipped_meleeWeapon != null)
        {
            //take what's left after melee_block, than use the proportion of DR_dex vs dex_adjusted
            melee_parryChance = Mathf.Clamp((1 - melee_blockChance) * (DR_dex/ dexSum), 0, 1);
        }
        else
        {
            melee_parryChance = 0f;
        }

        melee_dodgeChance = Mathf.Clamp(1 - (melee_blockChance + melee_dodgeChance), 0, 1);


        ranged_dodgeChance = Mathf.Clamp(1-ranged_blockChance, 0, 1);



        //calculate melee_AR
        float AR_str = str_adjusted;
        float AR_dex = dex_adjusted;
        if (equipped_meleeWeapon != null)
        {
            AR_str += str_adjusted * equipped_meleeWeapon.MeleeWeapon.attack_strBonusFactor;
            AR_dex += dex_adjusted * equipped_meleeWeapon.MeleeWeapon.attack_dexBonusFactor;
        }

        melee_attackRating = (AR_str + AR_dex) / 2;



        //calculate ranged_DR
        float ranged_DR_dex = dex_adjusted;
        if (equipped_shield != null)
        {
            ranged_DR_dex += dex_adjusted * equipped_shield.Shield.defense_dexBonusFactor;
        }

        ranged_defenseRating = ranged_DR_dex;

        //calculate ranged_AR
        float ranged_AR_dex = dex_adjusted;
        float ranged_AR_int = int_adjusted;

        if (equipped_rangedWeapon != null)
        {
            ranged_AR_dex += dex_adjusted * equipped_rangedWeapon.RangedWeapon.attack_dexBonusFactor;

            ranged_AR_int += int_adjusted * equipped_rangedWeapon.RangedWeapon.attack_intBonusFactor;
        }

        if (equipped_missile != null)
        {
            ranged_AR_dex += dex_adjusted * equipped_missile.Missile.attack_dexBonusFactor;

            ranged_AR_int += int_adjusted * equipped_missile.Missile.attack_intBonusFactor;
        }

        ranged_attackRating = (ranged_AR_dex + ranged_AR_int) / 2;

        degrees_of_accuracy = 1f / ((0.0015f * ranged_attackRating) + (1f / 180f));

        //Debug.Log($"{gameObject.name}: melee_DR {melee_defenseRating}, blockChance={blockChance}, dodgeChance={dodgeChance}, parryChance={parryChance}, melee_AR: {melee_attackRating}, ranged_AR={ranged_attackRating}");

        //calculate magic_DR
        float AR_will = will_adjusted;
        float DR_soul = soul_adjusted;
        magic_defenseRating = (will_adjusted + soul_adjusted) / 2;

        if (equipped_amulet != null)
        {
            magic_defenseRating *= (1 + equipped_amulet.Amulet.defensePhysical_modifier);
        }

        //calculate magic_AR
        float AR_int = int_adjusted;
        if (equipped_meleeWeapon != null)
        {
            AR_int += int_adjusted * equipped_meleeWeapon.MeleeWeapon.attack_intBonusFactor;
            AR_will += will_adjusted * equipped_meleeWeapon.MeleeWeapon.attack_willBonusFactor;
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


