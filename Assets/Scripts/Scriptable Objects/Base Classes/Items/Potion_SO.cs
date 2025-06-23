using UnityEngine;

[CreateAssetMenu(fileName = "Potion_SO", menuName = "Item/Potion_SO")]
public class Potion_SO : Item_SO
{
    [SerializeField] private PotionEffect_SO[] potionEffects;


    public void Use(GameObject target)
    {
        if (potionEffects.Length >0 && target != null)
        {
            foreach (var effect in potionEffects)
            {
                effect.Execute(target);

            }

        }

    }


}
