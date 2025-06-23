using UnityEngine;

//[CreateAssetMenu(fileName = "PotionEffect_SO", menuName = "Scriptable Objects/PotionEffect_SO")]
public abstract class PotionEffect_SO : ScriptableObject
{

    public abstract void Execute(GameObject target);

}
