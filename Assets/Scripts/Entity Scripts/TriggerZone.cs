using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public enum ZoneType { Far, Near }
    private Enemy_Behavior enemy;
    private ZoneType zone;


    private void Update()
    {
        CheckTriggerContents();
    }

    public void Setup(Enemy_Behavior enemyScript, ZoneType zoneType)
    {
        enemy = enemyScript;
        zone = zoneType;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            if (zone == ZoneType.Far)
            {
                enemy.SetFarTrigger(true);
            }
            else if (zone == ZoneType.Near)
            {
                enemy.SetNearTrigger(true);
            }
        }
    }

    private void CheckTriggerContents()
    {
        if (zone == ZoneType.Far)
        {
            bool farIsFilled = false;

            Collider[] collider_contents = Physics.OverlapSphere(transform.position, enemy.behaviorStats_SO.trigger_farRadius); // Adjust layer if needed

            foreach (Collider collider in collider_contents)
            {
                if (collider.CompareTag("Character"))
                {
                    farIsFilled = true;
                }
            }

            enemy.SetFarTriggerFill(farIsFilled);
        }
        else if (zone == ZoneType.Near)
        {
            bool nearIsFilled = false;

            Collider[] collider_contents = Physics.OverlapSphere(transform.position, enemy.behaviorStats_SO.trigger_nearRadius); // Adjust layer if needed

            foreach (Collider collider in collider_contents)
            {
                if (collider.CompareTag("Character"))
                {
                    nearIsFilled = true;
                }
            }

            enemy.SetNearTriggerFill(nearIsFilled);
        }
      

    }

    
}
