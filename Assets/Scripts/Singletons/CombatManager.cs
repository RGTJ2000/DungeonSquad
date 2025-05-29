using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.LightTransport.IProbeIntegrator;

public class CombatManager : MonoBehaviour
{
    // Singleton instance
    private static CombatManager _instance;
    public static CombatManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<CombatManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<CombatManager>();
                    singletonObject.name = typeof(CombatManager).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResolveMelee(GameObject attacker, GameObject target, bool critOverride = false)
    {
        Combat _combatOfDefender = target.GetComponent<Combat>();         // Get target's combat component

        EntityStats _attackerStats = attacker.GetComponent<EntityStats>();
        EntityStats _defenderStats = target.GetComponent<EntityStats>();

        //establish define variables
        float _attacker_meleeAR = _attackerStats.melee_attackRating;
        float _defender_meleeDR = _defenderStats.melee_defenseRating;

        Melee_Weapon_SO equippedMeleeWeapon = _attackerStats.equipped_meleeWeapon.MeleeWeapon;

        float damageBase = equippedMeleeWeapon.melee_damageBase;
        float damageRange = equippedMeleeWeapon.melee_damageRange;
        float critChance = equippedMeleeWeapon.critChance_base * (1 + _attackerStats.melee_critModifier);

        bool isHit = false;
        bool isCrit = false;

        // Determine if the attack hits
        float hitChance = CalculateHitChance(_attacker_meleeAR, _defender_meleeDR);
        float randomNum = Random.value;
        isHit = (Random.value < hitChance);
        //Debug.Log($"MELEE Hit Chance = {hitChance} | randomNum = {randomNum} | isHit = {isHit}");

        // Calculate damage if the attack hits
        List<DamageResult> damageList = new List<DamageResult>();
        CombatResultType resultType;

        if (isHit)
        {
            //calculate if crit
            float rndNum = Random.value;
            if (rndNum < critChance)
            {
                isCrit = true;
                resultType = CombatResultType.critical;
            }
            else
            {
                isCrit = false;
                resultType = CombatResultType.hit;
            }

            foreach (DamageStats damageStats in equippedMeleeWeapon.damageList)
            {
                float damageDone = damageStats.damage_base + Random.Range(0, damageStats.damage_range + 1);

                Debug.Log("Defender equipped armor=" + _defenderStats.equipped_armor);
                if (isCrit)
                {
                    damageDone *= 2f;

                }
                else if (_defenderStats.equipped_armor != null)
                {
                    damageDone -= (_defenderStats.equipped_armor.Armor.damageNegation_base + Random.Range(0, _defenderStats.equipped_armor.Armor.damageNegation_range + 1));
                }

                DamageResult damageResult = new DamageResult(damageStats.damageType, damageDone);
                damageList.Add(damageResult);

            }

            SoundManager.Instance.PlayVariationAtPosition(_attackerStats.equipped_meleeWeapon.MeleeWeapon.hitAudio_ID, attacker.transform.position, SoundCategory.sfx);
        }
        else
        {

            //figure out type of miss
            float rnd = Random.value;

            if (rnd < _defenderStats.melee_blockChance)
            {
                resultType = CombatResultType.block;
            }
            else if (rnd < _defenderStats.melee_dodgeChance + _defenderStats.melee_blockChance)
            {
                resultType = CombatResultType.dodge;
            }
            else
            {
                resultType = CombatResultType.parry;
            }


            SoundManager.Instance.PlayVariationAtPosition(_attackerStats.equipped_meleeWeapon.MeleeWeapon.missAudio_ID, attacker.transform.position, SoundCategory.sfx);
        }

        // Send the result to the target
        CombatResult combatResult = new CombatResult(attacker, resultType, damageList);
        _combatOfDefender.ReceiveCombatResult(combatResult);

    }

   
    public void ResolveMissile(MissileData _mData)
    {
        Combat _combatOfDefender = _mData.target.GetComponent<Combat>();         // Get target's combat component
        EntityStats _defenderStats = _mData.target.GetComponent<EntityStats>();

        float _defender_rangedDR = _defenderStats.ranged_defenseRating;

        bool isHit = false;
        bool isCrit = false;


        //calculate hit chance
        float hitChance = CalculateHitChance(_mData.attacker_rangedAR, _defender_rangedDR);

        float randomNum = Random.value;
        // Determine if the attack hits
        isHit = (Random.value < hitChance);


        // Calculate damage if the attack hits

        //float damageReceived = 0;

        List<DamageResult> damageList = new List<DamageResult>();
        CombatResultType resultType;

        if (isHit)
        {
            //calculate if crit
            float rndNum = Random.value;
            if (rndNum < _mData.critChance)
            {
                isCrit = true;
                resultType = CombatResultType.critical;
            }
            else
            {
                isCrit = false;
                resultType = CombatResultType.hit;
            }

            foreach (DamageStats damageStats in _mData.damageList)
            {
                float damageDone = damageStats.damage_base + Random.Range(0, damageStats.damage_range + 1);

                if (isCrit)
                {

                    damageDone *= 2f;


                }
                else if (_defenderStats.equipped_armor != null)
                {
                    damageDone -= (_defenderStats.equipped_armor.Armor.damageNegation_base + Random.Range(0, _defenderStats.equipped_armor.Armor.damageNegation_range + 1));
                }

                DamageResult damageResult = new DamageResult(damageStats.damageType, damageDone);
                damageList.Add(damageResult);

            }

        }
        else
        {
            //figure out type of miss
            float rnd = Random.value;

            if (rnd < _defenderStats.ranged_blockChance)
            {
                resultType = CombatResultType.block;
            }
            else
            {
                resultType = CombatResultType.dodge;
            }


        }

        CombatResult combatResult = new CombatResult(_mData.attacker, resultType, damageList);

        // Send the result to the target
        _combatOfDefender.ReceiveCombatResult(combatResult);
    }

    public void ResolveMagic(GameObject attacker, GameObject hit_object, string magicType, float damageBase, float damageRange, float magic_hitChanceMultiplier, float caster_magicAR)
    {


        Combat _combatOfDefender = hit_object.GetComponent<Combat>();
        StatusTracker _statusTracker = hit_object.GetComponent<StatusTracker>();
        //EntityStats _attackerStats = attacker.GetComponent<EntityStats>();
        EntityStats _defenderStats = hit_object.GetComponent<EntityStats>();

        float magic_AR = caster_magicAR;
        float magic_DR;
        float magic_damageMultiplier;

        switch (magicType)
        {
            case "confusion":
                magic_DR = _defenderStats.confusion_defenseRating;
                magic_damageMultiplier = _defenderStats.confusion_damageMultiplier;
                break;
            case "fear":
                magic_DR = _defenderStats.fear_defenseRating;
                magic_damageMultiplier = _defenderStats.fear_damageMultiplier;
                break;
            case "fire":
                magic_DR = _defenderStats.fire_defenseRating;
                magic_damageMultiplier = _defenderStats.fire_damageMultiplier;
                break;
            case "frost":
                magic_DR = _defenderStats.frost_defenseRating;
                magic_damageMultiplier = _defenderStats.frost_damageMultiplier;
                break;
            case "poison":
                magic_DR = _defenderStats.poison_defenseRating;
                magic_damageMultiplier = _defenderStats.poison_damageMultiplier;
                break;
            case "sleep":
                magic_DR = _defenderStats.sleep_defenseRating;
                magic_damageMultiplier = _defenderStats.sleep_damageMultiplier;
                break;
            case "physical":
                magic_DR = _defenderStats.ranged_defenseRating;
                magic_damageMultiplier = 1f;
                break;
            default:
                magic_DR = _defenderStats.ranged_defenseRating;
                magic_damageMultiplier = 1f;
                break;
        }

        float hitChance = (magic_AR * magic_AR) / ((magic_AR * magic_AR) + (magic_DR * magic_DR));
        hitChance = Mathf.Clamp01(hitChance * magic_hitChanceMultiplier);

        if (Random.value < hitChance)
        {
            float damage_received = (damageBase + Random.Range(0, damageRange + 1)) * magic_damageMultiplier;

            //_combatOfDefender.ReceiveCombatResult(true, magicType, damage_received, attacker);

            if (_statusTracker != null)
            {
                _statusTracker.ReceiveStatusCount(damage_received, magicType);

            }


        }


    }

    /*
    private float CalculateCritChance(EntityStats attackerStats)
    {
        float critChance = 0f;

        float str_w = attackerStats.equipped_meleeWeapon.MeleeWeapon.critChance_weight_str;

        float dex_w = attackerStats.equipped_meleeWeapon.MeleeWeapon.critChance_weight_dex;
        float int_w = attackerStats.equipped_meleeWeapon.MeleeWeapon.critChance_weight_int;
        float will_w = attackerStats.equipped_meleeWeapon.MeleeWeapon.critChance_weight_will;
        float soul_w = attackerStats.equipped_meleeWeapon.MeleeWeapon.critChance_weight_soul;

        float STR = attackerStats.strength;
        float DEX = attackerStats.dexterity;
        float INT = attackerStats.intelligence;
        float WLL = attackerStats.will;
        float SOL = attackerStats.soul;

        // This can increase the critChance by 2x at attribute=100. If attribute >100 then critchance keeps going up.
        critChance = attackerStats.equipped_meleeWeapon.MeleeWeapon.critChance_base * (1 + ((CritCalc(str_w, STR) + CritCalc(dex_w, DEX) + CritCalc(int_w, INT) + CritCalc(will_w, WLL) + CritCalc(soul_w, SOL)) / (str_w + dex_w + int_w + will_w + soul_w)));
        //Debug.Log($"Crit Chance = {critChance}");

        return critChance;
    }
    */

    private float CritCalc(float weight, float stat)
    {
        float calc = weight * ((stat - 50) / 50);
        return calc;
    }

    private float CalculateHitChance(float attackerAR, float defenderDR)
    {
        float AR_squared = attackerAR * attackerAR;
        float DR_squared = defenderDR * defenderDR;

        return (AR_squared) / (AR_squared + DR_squared);
    }

}
