using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomTargetSelect_SO", menuName = "Enemy AI/RandomTargetSelect_SO")]
public class RandomTargetSelect_SO : TargetSelection_SO
{
    public override GameObject Perform(GameObject attacker, List<GameObject> targets)
    {
        if (targets == null || targets.Count == 0) return null;

        int index = Random.Range(0, targets.Count);

        Debug.Log("TargetList count="+targets.Count+" index="+index+" Selected target=" + targets[index].name);
        return targets[index];

    }
}
