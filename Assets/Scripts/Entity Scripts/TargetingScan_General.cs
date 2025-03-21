using System.Collections.Generic;
using System;
using UnityEngine;

public class TargetingScan_General : MonoBehaviour
{
    private float visibleDistance = 50.0f;

    private GameObject[] visibleEnemies;
    public GameObject targeted_enemy = null;

 
    public GameObject[] ScanForVisibleTargets(string targetTag)
    {
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag(targetTag);

        List<GameObject> visibleTargetsList = new List<GameObject>();

        foreach (GameObject target in allTargets)
        {
            if (target.CompareTag(targetTag))
            {
                if (HitTargetBeforeWall(target))
                {
                    visibleTargetsList.Add(target);
                }

            }
        }

        GameObject[] visibleTargetsArray = visibleTargetsList.ToArray();


        if (visibleTargetsArray != null)
        {
            return visibleTargetsArray;
        }
        else
        {
            return null;
        }
    }


    public bool HitTargetBeforeWall(GameObject target)
    {
        RaycastHit[] hits_info = (Physics.RaycastAll(transform.position, target.transform.position - transform.position, visibleDistance));

        if (hits_info.Length > 0)
        {
            System.Array.Sort(hits_info, (a, b) => (a.distance.CompareTo(b.distance)));

            bool hit_wall = false;
            bool hit_target_before_wall = false;

            for (int i = 0; i < hits_info.Length; i++)
            {


                if (hits_info[i].transform.gameObject.CompareTag("Wall"))
                {
                    hit_wall = true;

                }

                if (!hit_wall && hits_info[i].transform.gameObject == target)
                {

                    hit_target_before_wall = true;

                }
            }

            return hit_target_before_wall;

        }
        else
        {
            return false; //target not hit at all
        }

    }
    /*
    public GameObject FindEnemyOnVector(GameObject[] enemy_array, Vector3 select_vector)
    {
        //set closest_enemy to the first enemy in array
        GameObject closest_enemy = enemy_array[0];

        foreach (GameObject enemy in enemy_array)
        {
            if (enemy != null)
            {


                //check if the current enemy angle is less than the closest_enemy angle, if so change closest enemy to current enemy
                if (Vector3.Angle(enemy.transform.position - transform.position, select_vector) < Vector3.Angle(closest_enemy.transform.position - transform.position, select_vector))
                {
                    closest_enemy = enemy;
                }
            }
        }


        return closest_enemy;
    }
    */

    public GameObject SetandReturnNearestTarget(string targetTag)
    {
        GameObject newNearestEnemy = FindNearestTarget(targetTag);
        targeted_enemy = newNearestEnemy;
        return newNearestEnemy;
    }

 
    public GameObject FindNearestTarget(string targetTag)
    {
        GameObject[] visible_target_array;

        visible_target_array = ScanForVisibleTargets(targetTag);
        if (visible_target_array.Length > 0)
        {
            Array.Sort(visible_target_array, (a, b) =>
            {
                float distanceA = Vector3.Distance(transform.position, a.transform.position);
                float distanceB = Vector3.Distance(transform.position, b.transform.position);

                return distanceA.CompareTo(distanceB); // Sort in ascending order
            });

            return visible_target_array[0];
        }
        else
        {
            return null;
        }


    }


}
