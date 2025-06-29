using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "TargetSelection_SO", menuName = "Enemy AI/TargetSelection_SO")]
public abstract class TargetSelection_SO : ScriptableObject
{
    public abstract GameObject Perform(GameObject attacker, List<GameObject> targets);


}
