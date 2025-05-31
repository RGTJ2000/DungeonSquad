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
                    damageDone -= Mathf.Max((_defenderStats.equipped_armor.Armor.damageNegation_base + Random.Range(0, _defenderStats.equipped_armor.Armor.damageNegation_range + 1)),0);
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
                    damageDone -= Mathf.Max((_defenderStats.equipped_armor.Armor.damageNegation_base + Random.Range(0, _defenderStats.equipped_armor.Armor.damageNegation_range + 1)),0);
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

    public void ResolveMagic(GameObject attacker, GameObject defender, List<DamageStats> damageList, bool alwaysHit, float caster_magicAR)
    {
        if (defender !=null)
        {
            Combat _combatOfDefender = defender.GetComponent<Combat>();
            StatusTracker _statusTracker = defender.GetComponent<StatusTracker>();
            EntityStats _defenderStats = defender.GetComponent<EntityStats>();

            float magic_AR = caster_magicAR;
            float defender_DR;
            float magic_damageMultiplier;
            CombatResultType resultType;

            List<DamageResult> damageResultList = new List<DamageResult>();

            bool isHit = false;

            //go through damage list and address each damage attempt
            foreach (DamageStats damageStats in damageList)
            {
                damageResultList.Clear();
                switch (damageStats.damageType)
                {
                    case DamageType.physical:
                        defender_DR = _defenderStats.magic_defenseRating;
                        magic_damageMultiplier = _defenderStats.magic_damageMultiplier;
                        break;
                    case DamageType.confusion:
                        defender_DR = _defenderStats.confusion_defenseRating;
                        magic_damageMultiplier = _defenderStats.confusion_damageMultiplier;
                        break;
                    case DamageType.fear:
                        defender_DR = _defenderStats.fear_defenseRating;
                        magic_damageMultiplier = _defenderStats.fear_damageMultiplier;
                        break;
                    case DamageType.fire:
                        defender_DR = _defenderStats.fire_defenseRating;
                        magic_damageMultiplier = _defenderStats.fire_damageMultiplier;
                        break;
                    case DamageType.frost:
                        defender_DR = _defenderStats.frost_defenseRating;
                        magic_damageMultiplier = _defenderStats.frost_damageMultiplier;
                        break;
                    case DamageType.poison:
                        defender_DR = _defenderStats.poison_defenseRating;
                        magic_damageMultiplier = _defenderStats.poison_damageMultiplier;
                        break;
                    case DamageType.sleep:
                        defender_DR = _defenderStats.sleep_defenseRating;
                        magic_damageMultiplier = _defenderStats.sleep_damageMultiplier;
                        break;
                    default:
                        defender_DR = _defenderStats.magic_defenseRating;
                        magic_damageMultiplier = _defenderStats.magic_damageMultiplier;
                        break;
                }


                if (alwaysHit)
                {
                    isHit = true;
                }
                else
                {
                    //calculate hit chance
                    float hitChance = CalculateHitChance(caster_magicAR, defender_DR);
                    // Determine if the attack hits
                    isHit = (Random.value < hitChance);
                }

                if (isHit)
                {
                    resultType = CombatResultType.hit;

                    float damageDone = (damageStats.damage_base + Random.Range(0, damageStats.damage_range + 1)) * magic_damageMultiplier;

                    DamageResult damageResult = new DamageResult(damageStats.damageType, damageDone);
                    damageResultList.Add(damageResult);
                }
                else
                {
                    resultType = CombatResultType.resist;
                }

                CombatResult combatResult = new CombatResult(attacker, resultType, damageResultList);

                // Send the result to the target
                _combatOfDefender.ReceiveCombatResult(combatResult);


            }
        }
    }

  

 

    private float CalculateHitChance(float attackerAR, float defenderDR)
    {
        float AR_squared = attackerAR * attackerAR;
        float DR_squared = defenderDR * defenderDR;

        return (AR_squared) / (AR_squared + DR_squared);
    }

}
