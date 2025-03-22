using UnityEngine;

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

        float attackRating = _attackerStats.melee_attackRating;
        float damageBase = _attackerStats.equipped_meleeWeapon.melee_damageBase;
        float damageRange = _attackerStats.equipped_meleeWeapon.melee_damageRange;
        float critChance;

        float defenseRating = _defenderStats.melee_defenseRating;

        bool isHit = false;
        string damageType = "physical";

        //Debug.Log($"{attacker.name} attacking {target.name}");

        // Calculate hit chance (example formula, adjust as needed)
        //float hitChance = Mathf.Clamp01(attackRating / (attackRating + defenseRating));
        float hitChance = (attackRating * attackRating) / ((attackRating * attackRating) + (defenseRating * defenseRating));


        // Determine if the attack hits
        float randomNum = Random.value;
        isHit = (Random.value < hitChance);

        //Debug.Log($"Hit Chance = {hitChance} | randomNum = {randomNum} | isHit = {isHit}");

        // Calculate damage if the attack hits
        float damageReceived = 0;
        if (isHit)
        {

            damageReceived = damageBase + Random.Range(0, damageRange + 1);

            //calculate if crit
            critChance = CalculateCritChance(_attackerStats);
            float rndNum = Random.value;
            //Debug.Log($"CritChance= {critChance} Random Crit num = {rndNum}");
            if (rndNum < critChance || critOverride) 
            {
                damageType = "crit";
                damageReceived = damageReceived * 2;
                //Debug.Log("CRIT. Damage x2");
            }
            //Debug.Log($"Damage Received = {damageReceived}");
        }

        // Send the result to the target
        _combatOfDefender.ReceiveCombatResult(isHit, damageType, damageReceived, attacker);
    }

    public void ResolveMissile(GameObject attacker, GameObject target, Weapon_SO missile_SO, float missile_AR, float missile_critChance)
    {
        Combat _combatOfDefender = target.GetComponent<Combat>();         // Get target's combat component
        //EntityStats _attackerStats = attacker.GetComponent<EntityStats>();
        EntityStats _defenderStats = target.GetComponent<EntityStats>();

        float attackRating = missile_AR;
        float damageBase = missile_SO.missile_damageBase;
        float damageRange = missile_SO.missile_damageRange;
        float critChance = missile_critChance;

        bool isHit = false;

        float defenseRating = _defenderStats.ranged_defenseRating;
        string damageType = "physical";
       
        //Debug.Log($"Missile: {missile_SO.weaponName} hitting {target.name} | missileAr= {missile_AR} missileCritChance {missile_critChance}");

        // Calculate hit chance (example formula, adjust as needed)
        //float hitChance = Mathf.Clamp01(attackRating / (attackRating + defenseRating));
        float hitChance = (attackRating * attackRating) / ((attackRating * attackRating) + (defenseRating * defenseRating));

        float randomNum = Random.value;
        // Determine if the attack hits
        isHit = (Random.value < hitChance);


        // Calculate damage if the attack hits
        float damageReceived = 0;
        if (isHit)
        {

            damageReceived = damageBase + Random.Range(0, damageRange + 1);

            //calculate if crit
            float rndNum = Random.value;
            //Debug.Log($"Random Crit num = {rndNum}");
            if (rndNum < critChance)
            {
                damageType = "crit";
                damageReceived = damageReceived * 2;
            }
        }

        // Send the result to the target
        _combatOfDefender.ReceiveCombatResult(isHit, damageType, damageReceived, attacker);
    }

    public void ResolveMagic(GameObject attacker, GameObject hit_object, string magicType, float damageBase, float damageRange, float magic_hitChanceMultiplier, float caster_magicAR)
    {


        Combat _combatOfDefender = hit_object.GetComponent<Combat>();
        StatusTracker _statusTracker = hit_object.GetComponent<StatusTracker>();
        //EntityStats _attackerStats = attacker.GetComponent<EntityStats>();
        EntityStats _defenderStats = hit_object.GetComponent<EntityStats> ();

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
            _combatOfDefender.ReceiveCombatResult(true, magicType, damage_received, attacker);
            if (_statusTracker != null)
            {
                _statusTracker.ReceiveStatusCount(damage_received, magicType);

            }


        }


    }

    private float CalculateCritChance(EntityStats attackerStats)
    {
        float critChance = 0f;

        float str_w = attackerStats.equipped_meleeWeapon.critChance_weight_str;

        float dex_w = attackerStats.equipped_meleeWeapon.critChance_weight_dex;
        float int_w = attackerStats.equipped_meleeWeapon.critChance_weight_int;
        float will_w = attackerStats.equipped_meleeWeapon.critChance_weight_will;
        float soul_w = attackerStats.equipped_meleeWeapon.critChance_weight_soul;

        float STR = attackerStats.strength;
        float DEX = attackerStats.dexterity;
        float INT = attackerStats.intelligence;
        float WLL = attackerStats.will;
        float SOL = attackerStats.soul;

        critChance = attackerStats.equipped_meleeWeapon.critChance_base * ( 1 + ( ( CritCalc(str_w,STR)+CritCalc(dex_w,DEX)+CritCalc(int_w, INT)+CritCalc(will_w, WLL)+ CritCalc(soul_w, SOL)) / (str_w + dex_w + int_w + will_w + soul_w ) ) );
        //Debug.Log($"Crit Chance = {critChance}");

        return critChance;
    }

    private float CritCalc(float weight, float stat)
    {
        float calc = weight * ((stat - 50) / 50);
        return calc;
    }

}
