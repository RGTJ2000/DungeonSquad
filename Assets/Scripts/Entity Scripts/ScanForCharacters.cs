using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ScanForCharacters : MonoBehaviour
{

    private GameObject[] visible_characters;
    public GameObject selected_character = null;

    private EntityStats _entitystats;

    public GameObject targeted_character = null;


    void Start()
    {
        _entitystats = GetComponent<EntityStats>();
    }

    private void Update()
    {

    }

    public GameObject[] ScanVisibleCharacters(float radius)
    {
        GameObject[] allCharacters = GameObject.FindGameObjectsWithTag("Character");

        List<GameObject> visibleCharactersList = new List<GameObject>();

        foreach (GameObject character in allCharacters)
        {

            RaycastHit[] hits_info = (Physics.RaycastAll(transform.position, character.transform.position - transform.position, radius));

            if (hits_info.Length > 0)
            {
                System.Array.Sort(hits_info, (a, b) => (a.distance.CompareTo(b.distance)));

                bool hit_wall = false;
                bool hit_ch_before_wall = false;

                for (int i = 0; i < hits_info.Length; i++)
                {


                    if (hits_info[i].transform.gameObject.CompareTag("Wall"))
                    {
                        hit_wall = true;

                    }

                    if (!hit_wall && hits_info[i].transform.gameObject.CompareTag("Character"))
                    {

                        hit_ch_before_wall = true;
                    }
                }

                if (hit_ch_before_wall)
                {

                    visibleCharactersList.Add(character);
                }

            }



        }

        GameObject[] visibleCharactersArray = visibleCharactersList.ToArray();


        if (visibleCharactersArray != null)
        {
            return visibleCharactersArray;
        }
        else
        {
            return null;
        }
    }


    public GameObject FindNearestVisibleCharacter(float radius)
    {
        GameObject[] visible_ch_array;

        visible_ch_array = ScanVisibleCharacters(radius);

        if (visible_ch_array.Length > 0)
        {
            Array.Sort(visible_ch_array, (a, b) =>
            {
                float distanceA = Vector3.Distance(transform.position, a.transform.position);
                float distanceB = Vector3.Distance(transform.position, b.transform.position);

                return distanceA.CompareTo(distanceB); // Sort in ascending order
            });

            return visible_ch_array[0]; //returns closest visible character
        }
        else
        {
            return null; //if no charcters visible, return null
        }


    }


    public bool CheckCharacterIsVisible(GameObject ch_obj, float radius)
    {
        bool isVisible = false;

        RaycastHit[] hits_info = (Physics.RaycastAll(transform.position, ch_obj.transform.position - transform.position, radius));

        if (hits_info.Length > 0)
        {
            System.Array.Sort(hits_info, (a, b) => (a.distance.CompareTo(b.distance)));

            bool hit_wall = false;
            bool hit_ch_before_wall = false;

            for (int i = 0; i < hits_info.Length; i++)
            {


                if (hits_info[i].transform.gameObject.CompareTag("Wall"))
                {
                    hit_wall = true;

                }

                if (!hit_wall && hits_info[i].transform.gameObject == ch_obj)
                {

                    hit_ch_before_wall = true;
                }
            }

            if (hit_ch_before_wall)
            {
                isVisible = true;
            } 

        }

        return isVisible;

    }


    public GameObject SetAndReturnNearestCharacter(float radius)
    {
        GameObject newNearestCharacter = FindNearestVisibleCharacter(radius);
        targeted_character = newNearestCharacter;
        return newNearestCharacter;
    }

    public void ChangeTargetedCharacter(GameObject attacker)
    {
        targeted_character = attacker;
    }

    public GameObject GetTargetedCharacter()
    {
        return targeted_character;
    }
}
