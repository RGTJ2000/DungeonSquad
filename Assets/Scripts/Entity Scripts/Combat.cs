using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class Combat : MonoBehaviour
{
    private Health _health;
    private FloatTextDisplay _floatTextDisplay;
    private StatusTracker _statusTracker;
    private ThreatTracker _threatTracker;
    private Enemy_Behavior2 _enemyBehavior;


    void Start()
    {
        _health = GetComponent<Health>();
        _floatTextDisplay = GetComponent<FloatTextDisplay>();
        _statusTracker = GetComponent<StatusTracker>();
        _threatTracker = GetComponent<ThreatTracker>();
        _enemyBehavior = GetComponent<Enemy_Behavior2>();

    }
    

    public void ReceiveCombatResult(CombatResult combat_result)
    {


        if (combat_result.resultType == CombatResultType.hit || combat_result.resultType == CombatResultType.critical)
        {
            foreach (DamageResult damageResult in combat_result.damageResultList)
            {
                //show the damage as floating text
                _floatTextDisplay.ShowFloatDamage(damageResult.DamageAmount, combat_result.resultType, damageResult.DamageType);

                if (_threatTracker != null)
                {
                    _threatTracker.ReceiveDamage(combat_result.attacker, damageResult.DamageAmount);
                }

                if (_enemyBehavior != null)
                {
                    _enemyBehavior.OnDamageCheckTargetedCharacter(combat_result.attacker);

                }

                //allot the damage to either health or status
                switch (damageResult.DamageType)
                {
                    case DamageType.physical:
                        // Handle physical damage
                        _health.TakeDamage(damageResult.DamageAmount);
                        

                        break;

                    case DamageType.confusion:
                        // Handle confusion effect
                        _statusTracker.ReceiveStatusCount(damageResult.DamageAmount, damageResult.DamageType);
                        //add points to statusTracker
                        break;

                    case DamageType.fear:
                        // Handle fear effect
                        //add points to statusTracker
                        _statusTracker.ReceiveStatusCount(damageResult.DamageAmount, damageResult.DamageType);
                        break;

                    case DamageType.fire:
                        // Handle fire damage (maybe apply burning)
                        //add points to statusTracker
                        _health.TakeDamage(damageResult.DamageAmount);
                        _statusTracker.ReceiveStatusCount(damageResult.DamageAmount, damageResult.DamageType);
                        
                        break;

                    case DamageType.frost:
                        // Handle frost damage (maybe slow target)
                        //add points to statusTracker
                        _statusTracker.ReceiveStatusCount(damageResult.DamageAmount, damageResult.DamageType);
                       
                        break;

                    case DamageType.poison:
                        // Handle poison damage (maybe apply DoT)
                        //add points to statusTracker
                        _statusTracker.ReceiveStatusCount(damageResult.DamageAmount, damageResult.DamageType);
                        break;

                    case DamageType.sleep:
                        // Handle sleep effect (maybe disable actions)
                        //add points to statusTracker
                        _statusTracker.ReceiveStatusCount(damageResult.DamageAmount, damageResult.DamageType);
                        break;

                    default:
                        // Optional: catch unexpected or unhandled types
                        Debug.LogWarning("Unhandled damage type: " + damageResult.DamageType);
                        break;

                }


            }

        }
        else
        {
            //_health.Miss();
            _floatTextDisplay.ShowFloatMiss(combat_result.resultType);
        }




    }



}
