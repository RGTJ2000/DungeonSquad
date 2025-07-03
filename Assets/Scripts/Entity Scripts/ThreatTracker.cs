using System.Collections.Generic;
using UnityEngine;



public struct AttackerAndThreat
{
    public GameObject attacker;
    public float threatLevel;

    public AttackerAndThreat(GameObject attacker, float threatLevel)
    {
        this.attacker = attacker;
        this.threatLevel = threatLevel;
    }
}

public class ThreatTracker : MonoBehaviour
{

    private float dpsWindow;
    private float damageBufferWeight;
    private float damageBufferDecay;

    [SerializeField] private GameObject threatDisplay_prefab;

    private EntityStats _entityStats;

    public struct AttackerAndThreat
    {
        public GameObject attacker;
        public float threatLevel;

        public AttackerAndThreat(GameObject attacker, float threatLevel)
        {
            this.attacker = attacker;
            this.threatLevel = threatLevel;
        }
    }

    // --- One attacker’s threat data ---
    public class ThreatData
    {
        public List<DamageEvent> damageEvents = new();
        public float damageBuffer = 0f;
        public float totalDamage = 0f;
    }

    public struct DamageEvent
    {
        public float damageAmount;
        public float time;

        public DamageEvent(float amount, float time)
        {
            this.damageAmount = amount;
            this.time = time;
        }
    }

    private Dictionary<GameObject, ThreatData> threatTable = new();


    private void Start()
    {
        _entityStats = GetComponent<EntityStats>();


        damageBufferDecay = _entityStats.health_max * 0.05f;

        damageBufferWeight = 0.3f;
        dpsWindow = 3f;
    }

    private void Update()
    {

        float delta = Time.deltaTime;

        foreach (var kv in threatTable)
        {
            Debug.Log("damage decay=" + damageBufferDecay);
            kv.Value.damageBuffer -= damageBufferDecay * delta;
            kv.Value.damageBuffer = Mathf.Max(0f, kv.Value.damageBuffer);
        }

    }

  

    //PUBLIC METHODS

    public void ReceiveDamage(GameObject attacker, float damageAmount)
    {
        if (!threatTable.ContainsKey(attacker))
        {
            threatTable[attacker] = new ThreatData();

            GameObject newDisplay = Instantiate(threatDisplay_prefab);
            ThreatDisplayBehavior _behaviour = newDisplay.GetComponent<ThreatDisplayBehavior>();
            _behaviour.SetObjectToTrack(attacker, this);
        }

        var data = threatTable[attacker];

        data.damageEvents.Add(new DamageEvent(damageAmount, Time.time));
        data.damageBuffer += damageAmount;
        data.totalDamage += damageAmount;

    }

    public GameObject GetTopAttackerByThreatLevel()
    {
        GameObject topAttacker = null;
        float highestThreatLevel = 0f;

        foreach (var kv in threatTable)
        {

            float threatLevel = GetThreatLevel(kv.Key);
            if (threatLevel > highestThreatLevel)
            {
                highestThreatLevel = threatLevel;
                topAttacker = kv.Key;
            }
        }

        return topAttacker;
    }


    public float GetThreatLevel(GameObject attacker)
    {
        if (!threatTable.ContainsKey(attacker))
            return 0f;

        var data = threatTable[attacker];
        float now = Time.time;

        data.damageEvents.RemoveAll(e => now - e.time > dpsWindow);

        float total = 0f;

        foreach (var e in data.damageEvents)
        {
            total += e.damageAmount;
        }

        float dps = total / dpsWindow;

        float threatLevel = dps + (damageBufferWeight * data.damageBuffer);

        return threatLevel;

    }




    public float GetDPS(GameObject attacker)
    {
        if (!threatTable.ContainsKey(attacker))
            return 0f;

        var data = threatTable[attacker];
        float now = Time.time;

        data.damageEvents.RemoveAll(e => now - e.time > dpsWindow);

        float total = 0f;
        foreach (var e in data.damageEvents)
        {
            total += e.damageAmount;
        }


        return total / dpsWindow;
    }

    // --- Get total damage for an attacker ---
    public float GetTotalDamage(GameObject attacker)
    {
        if (!threatTable.ContainsKey(attacker))
            return 0f;

        return threatTable[attacker].totalDamage;
    }

    // --- Get top threat by total damage ---
    public GameObject GetTopThreatByTotalDamage()
    {
        GameObject topAttacker = null;
        float highestTotal = 0f;

        foreach (var kv in threatTable)
        {
            float total = kv.Value.totalDamage;
            if (total > highestTotal)
            {
                highestTotal = total;
                topAttacker = kv.Key;
            }
        }

        return topAttacker;
    }



    public AttackerAndThreat GetTopAttackerAndThreatLevel()
    {
        GameObject topAttacker = GetTopAttackerByThreatLevel();
        float threatLevel = GetThreatLevel(topAttacker);

        AttackerAndThreat attackerAndThreat = new AttackerAndThreat(topAttacker, threatLevel);

        return attackerAndThreat;

    }

    // Optional: get all attackers and their DPS
    public Dictionary<GameObject, float> GetAllDPS()
    {
        var result = new Dictionary<GameObject, float>();
        foreach (var kv in threatTable)
        {
            result[kv.Key] = GetDPS(kv.Key);
        }
        return result;
    }

}
