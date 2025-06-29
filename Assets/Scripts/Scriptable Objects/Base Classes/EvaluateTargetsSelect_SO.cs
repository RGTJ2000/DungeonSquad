using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "EvaluateTargetsSelect_SO", menuName = "Enemy AI/EvaluateTargetsSelect_SO")]
public class EvaluateTargetsSelect_SO : TargetSelection_SO
{

    public List<StatType> statsToEvaluate;
    public bool statTargetingOn = false;
    public bool findStatHigh = true;
    public float statEvalWeighting = 1.0f;

    public bool armorTargetingOn = false;
    public bool findArmorHigh = true;
    public float armorEvalWeighting = 1.0f;

    public bool healthTargetingOn = false;
    public bool findHealthHigh = true;
    public float healthEvalWeighting = 1.0f;

    public class TargetAndScore
    {
        public GameObject target;
        public float score;

        public TargetAndScore(GameObject target, float score)
        {
            this.target = target;
            this.score = score;
        }
    }
   

    public override GameObject Perform(GameObject attacker, List<GameObject> targets)
    {

        if (targets.Count == 0) return null;


        GameObject selectedTarget = null;



        List<TargetAndScore> TargetScoreList = new List<TargetAndScore>();
        List<TargetAndScore> SelectedTargetsList = new List<TargetAndScore>();


        if (statTargetingOn)
        {

            TargetScoreList.Clear();
            //populate TargetScoreList
            foreach (GameObject _target in targets)
            {
                float currentScore = StatScoreForTarget(_target);
                TargetScoreList.Add(new TargetAndScore(_target, currentScore));
            }

            ScaleScores(TargetScoreList, statEvalWeighting, attacker);

            //get the highest target with score, random if scores equal
            TargetAndScore selectedForStats = GetHighestScoreRandom(TargetScoreList);

            if (selectedForStats != null)
            {
                SelectedTargetsList.Add(selectedForStats);

            }

        }

        if (healthTargetingOn)
        {
            TargetScoreList.Clear();
            
            foreach (GameObject _target in targets)
            {
                Health _health = _target.GetComponent<Health>();
                TargetScoreList.Add(new TargetAndScore(_target, _health.currentHealth));
            }

            ScaleScores(TargetScoreList, healthEvalWeighting, attacker);

            //get the highest target with score, random if scores equal
            TargetAndScore selectedForHealth = GetHighestScoreRandom(TargetScoreList);

            if (selectedForHealth != null)
            {
                SelectedTargetsList.Add(selectedForHealth);

            }



        }

        if (armorTargetingOn)
        {
            TargetScoreList.Clear();

            foreach (GameObject _target in targets)
            {
                EntityStats _entityStats = _target.GetComponent<EntityStats>();

                float armorNegation;

                if (_entityStats.equipped_armor == null)
                {
                    armorNegation = 0f;
                }
                else
                {
                    armorNegation = _entityStats.equipped_armor.Armor.damageNegation_base + (0.5f * _entityStats.equipped_armor.Armor.damageNegation_range);
                }

                TargetScoreList.Add(new TargetAndScore (_target, armorNegation));
            }

            ScaleScores(TargetScoreList, armorEvalWeighting, attacker);

            //get the highest target with score, random if scores equal
            TargetAndScore selectedForArmor = GetHighestScoreRandom(TargetScoreList);

            if (selectedForArmor != null)
            {
                SelectedTargetsList.Add(selectedForArmor);

            }


        }


        //compare the scores for all the active categories and select the target with the highest score

        selectedTarget = GetHighestScoreRandom(SelectedTargetsList).target;


        return selectedTarget;
    }


    private float StatScoreForTarget(GameObject target)
    {
        float sumOfStats = 0f;

        EntityStats entityStats = target.GetComponent<EntityStats>();

        foreach (StatType statType in statsToEvaluate)
        {
            float currentStat;

            switch (statType)
            {
                case StatType.strength: currentStat = entityStats.str_adjusted; break;
                case StatType.dexterity: currentStat = entityStats.dex_adjusted; break;
                case StatType.intelligence: currentStat = entityStats.intelligence; break;
                case StatType.will: currentStat = entityStats.will; break;
                case StatType.soul: currentStat = entityStats.soul; break;
                default: currentStat = 0f; break;
            }

            sumOfStats += currentStat;

        }

        float averageOfStats = sumOfStats / (float)statsToEvaluate.Count;


        return averageOfStats;
    }

    private TargetAndScore SelectTargetFromScore(TargetAndScore target1, TargetAndScore target2)
    {
        

        if (target1.target == null &&  target2.target == null)
        {
            return target1;
        }
        else if (target1.target == null && target2.target != null)
        {
            return target2;
        }
        else if (target1.target  != null && target2.target == null)
        {
            return target1;
        }
        else if (target1.score > target2.score)
        {
            return target1;
        }
        else if (target2.score > target1.score)
        {
            return target2;
        }
        else if (target1.score == target2.score)
        {
            if (Random.value > 0.5f)
            {
                return target1;
            }
            else
            {
                return target2;
            }
        }
        else
        {
            return target1;
        }




    }


    private void ScaleScores(List<TargetAndScore> targets, float weighting, GameObject attacker)
    {
        float lowScore = Mathf.Infinity;

        //find low score as a base
        foreach (TargetAndScore _target in targets)
        {
            if (_target.score < lowScore)
            {
                lowScore = _target.score;
            }
        }

        foreach (TargetAndScore _target in targets)
        {
            float scaledScore = ( (_target.score / lowScore) * weighting ) / (attacker.transform.position - _target.target.transform.position).magnitude;

            _target.score = scaledScore;

        }



    }

    private TargetAndScore GetHighestScoreRandom(List<TargetAndScore> list)
    {
        if (list == null || list.Count == 0) return null;

        // Find max score
        float maxScore = list.Max(c => c.score);

        // Filter to those with max score
        List<TargetAndScore> highest = list.Where(c => c.score == maxScore).ToList();

        // Pick one at random
        int index = Random.Range(0, highest.Count);

        return highest[index];




    }

}
