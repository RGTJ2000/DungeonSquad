using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TargetingScan : MonoBehaviour
{
    private float visibleDistance = 50.0f;


    public bool scanningOn = false;             //toggles scanning lines
    private GameObject[] visibleTargets;
    public GameObject targeted_entity = null;
    public GameObject highlighted_target = null;


    public Material red_line_mat;
    public Material green_line_mat;
    public Material blue_line_mat;
    public Material yellow_line_mat;

    public Material[] lineMaterialArray;
    public Material character_lineMaterial;



    private EntityStats _entityStats;

    private LineRenderer _lineRenderer_targetArrow;

    public bool target_arrow_on = false;

    private bool newSelectionMade = false;


    void Start()
    {


        _entityStats = GetComponent<EntityStats>();
        lineMaterialArray = new Material[] { red_line_mat, green_line_mat, blue_line_mat, yellow_line_mat };
        character_lineMaterial = lineMaterialArray[_entityStats.character_ID];

        // Add and configure LineRenderer at runtime
       
        GameObject arrowObj = new GameObject("TargetArrowLineRenderer");
        arrowObj.transform.SetParent(transform, false); // Parent to this object, local position zero
        _lineRenderer_targetArrow = arrowObj.AddComponent<LineRenderer>();

        _lineRenderer_targetArrow.enabled = true;
        _lineRenderer_targetArrow.useWorldSpace = true; // Default for your arrow
        _lineRenderer_targetArrow.positionCount = 0; // Start with no positions

    }

    private void Update()
    {
        // Find all GameObjects with the "Enemy" tag and store them in allEnemies
        // allEnemies = GameObject.FindGameObjectsWithTag("Enemy");


        if (scanningOn)
        {
            bool highlighted_target_match = false;

            visibleTargets = ScanForVisibleTargets();


            if (visibleTargets.Length > 0)
            {
                //if no highlighted target, find target object with vector angle closest to transform.forward position
                if (highlighted_target == null)
                {
                    highlighted_target = FindTargetOnVector(visibleTargets, transform.forward);

                }


                for (int i = 0; i < visibleTargets.Length; i++)
                {
                    //step through visible targets, turn on target active_line and set this gameobject as target
                    ReturnLinePlot currentTarget_lineplot = visibleTargets[i].GetComponent<ReturnLinePlot>();
                    currentTarget_lineplot.visibleDistance = visibleDistance;
                    currentTarget_lineplot.active_line = true;
                    currentTarget_lineplot.target_obj = this.gameObject;

                    currentTarget_lineplot.SetLineMaterial(character_lineMaterial);

                    //LineRenderer target_linerenderer = visibleTargets[i].GetComponent<LineRenderer>();
                    //target_linerenderer.material = character_lineMaterial;

                    if (visibleTargets[i] == highlighted_target) //if the highlighted target, make the return line wider
                    {
                        currentTarget_lineplot.SetReturnLineWidth(0.6f, 0.3f);
                        /*
                           target_linerenderer.startWidth = 0.3f;
                           target_linerenderer.endWidth = 0.6f;
                        */
                        highlighted_target_match = true; //only set to true if existing highlighted enemy is still visible
                    }
                    else // if not the highlighted target, make the line thinner
                    {
                        currentTarget_lineplot.SetReturnLineWidth(0.1f, 0.1f);
                        /*
                        target_linerenderer.startWidth = 0.1f;
                        target_linerenderer.endWidth = 0.1f;
                        */
                    }
                    //the return scan will then be active and tracking character, if line of sight breaks, target turns off the return line itself
                }

                if (!highlighted_target_match) //if the existing highlighted enemy not visible, then reset the highlight to null so that it will be refound by FindTargetOnVector
                {
                    highlighted_target = null;
                }

            }
        }
        else
        {
            visibleTargets = null; //no visible targets found
            highlighted_target = null;

            //disable all return lines

        }

        if (target_arrow_on)
        {
            if (targeted_entity != null)
            {
                
                PlotTargetArrow(targeted_entity);
            }
            else
            {
                DeactivateTargetArrow(); //turn off arrow if it's on an target is null
            }


        }


    }


    public GameObject[] ScanForVisibleTargets()
    {
        Targeting_Type type = _entityStats.selected_skill.skill_targetType;
        //string type = _entityStats.selected_skill.skill_targetType;
        string targetingTag = "none";

        switch (type)
        {
            case Targeting_Type.self:
                break;
            case Targeting_Type.group:
                targetingTag = "Character";
                break;
            case Targeting_Type.other:
                targetingTag = "Enemy";
                break;
            case Targeting_Type.area:
                break;
            default:
                targetingTag = "none";
                break;
        }


        GameObject[] allTargets = GameObject.FindGameObjectsWithTag(targetingTag);

        List<GameObject> visibleTargetsList = new List<GameObject>();

        foreach (GameObject target in allTargets)
        {
            if (target.CompareTag(targetingTag))
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

    public GameObject FindTargetOnVector(GameObject[] target_array, Vector3 select_vector)
    {
        //set closest_enemy to the first enemy in array
        GameObject closest_target = target_array[0];

        foreach (GameObject target in target_array)
        {
            if (target != null)
            {


                //check if the current target angle is less than the closest_target angle, if so change closest angle target to current target
                if (Vector3.Angle(target.transform.position - transform.position, select_vector) < Vector3.Angle(closest_target.transform.position - transform.position, select_vector))
                {
                    closest_target = target;
                }
            }
        }


        return closest_target;
    }

    public GameObject SetandReturnNearestTargetEntity()
    {
        GameObject newNearestTargetEntity = FindNearestEntity_withTag();
        targeted_entity = newNearestTargetEntity;
        return newNearestTargetEntity;
    }

    public void SelectNewEntity(Vector2 vector_input)
    {
        if (vector_input == Vector2.zero)
        {
            newSelectionMade = false;
        }
        else
        {

            Vector3 selection_vector = new Vector3(vector_input.x, 0f, vector_input.y);

            if (scanningOn && visibleTargets != null && visibleTargets.Length > 0)
            {
                if (highlighted_target == null)
                {
                    highlighted_target = FindTargetOnVector(visibleTargets, selection_vector);
                    newSelectionMade = true;
                }
                else
                {
                    if (!newSelectionMade)
                    {
                        //sort visibleTargets by degrees from highlighted target vector, use Vector3.SignedAngle

                        Vector3 highlighted_targetVector = (highlighted_target.transform.position - transform.position).normalized;


                        var sortedAngleArrays = SortTargetsByAngle(visibleTargets, highlighted_targetVector);

                        GameObject[] entitiesOnNegativeAngles = sortedAngleArrays.Item1;
                        GameObject[] entitiesOnPositiveAngles = sortedAngleArrays.Item2;

                        float highlight_to_selection_angle = Vector3.SignedAngle(highlighted_targetVector, selection_vector, Vector3.up);

                        if (highlight_to_selection_angle < -20f && highlight_to_selection_angle >= -135f && entitiesOnNegativeAngles.Length > 0)
                        {
                            SoundManager.Instance.PlaySoundByKey("single_click", SoundCategory.UI);

                            highlighted_target = entitiesOnNegativeAngles[0];

                            newSelectionMade = true;

                        }
                        else if (highlight_to_selection_angle > 20f && highlight_to_selection_angle <= 135f && entitiesOnPositiveAngles.Length > 0)
                        {
                            SoundManager.Instance.PlaySoundByKey("single_click", SoundCategory.UI);

                            highlighted_target = entitiesOnPositiveAngles[0];
                            newSelectionMade = true;
                        }
                        else if (highlight_to_selection_angle < -135f || highlight_to_selection_angle > 135f)
                        {
                            SoundManager.Instance.PlaySoundByKey("single_click", SoundCategory.UI);

                            if (entitiesOnNegativeAngles.Length > 0 && entitiesOnPositiveAngles.Length > 0)
                            {

                                float neg_angle = Vector3.Angle(highlighted_targetVector, entitiesOnNegativeAngles[entitiesOnNegativeAngles.Length - 1].transform.position - transform.position);
                                float pos_angle = Vector3.Angle(highlighted_targetVector, entitiesOnPositiveAngles[entitiesOnPositiveAngles.Length - 1].transform.position - transform.position);

                                if (neg_angle >= pos_angle)
                                {
                                    highlighted_target = entitiesOnNegativeAngles[entitiesOnNegativeAngles.Length - 1];
                                    newSelectionMade = true;

                                }
                                else
                                {
                                    highlighted_target = entitiesOnPositiveAngles[entitiesOnPositiveAngles.Length - 1];
                                    newSelectionMade = true;

                                }

                            }
                            else if (entitiesOnNegativeAngles.Length > 0)
                            {
                                highlighted_target = entitiesOnNegativeAngles[entitiesOnNegativeAngles.Length - 1];
                                newSelectionMade = true;
                            }
                            else if (entitiesOnPositiveAngles.Length > 0)
                            {
                                highlighted_target = entitiesOnPositiveAngles[entitiesOnPositiveAngles.Length - 1];
                                newSelectionMade = true;
                            }
                        }

                    }



                }


              
            }
        }
    }

    private (GameObject[], GameObject[]) SortTargetsByAngle(GameObject[] targets_array, Vector3 reference_vector)
    {
        List<Tuple<GameObject, float>> negative_list = new List<Tuple<GameObject, float>>();
        List<Tuple<GameObject, float>> positive_list = new List<Tuple<GameObject, float>>();


        for (int i = 0; i < targets_array.Length; i++)
        {
            float angleValue;

            angleValue = Vector3.SignedAngle(reference_vector, (targets_array[i].transform.position - transform.position).normalized, Vector3.up);

            if (angleValue < 0) //excluse 0 because that will be the currently highlighted entity
            {
                negative_list.Add(Tuple.Create(targets_array[i], Mathf.Abs(angleValue)));
            }
            else if (angleValue>0)
            {
                positive_list.Add(Tuple.Create(targets_array[i], Mathf.Abs(angleValue)));
            }

        }

        //sort the lists by their angleValue (item2)
        negative_list.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));
        positive_list.Sort((t1, t2) => t1.Item2.CompareTo(t2.Item2));

        //assign the list item1's to arrays
        GameObject[] negative_array = negative_list.Select(t => t.Item1).ToArray();
        GameObject[] positive_array = positive_list.Select(t => t.Item1).ToArray();

        return (negative_array, positive_array);
    }

    public void ResetNewSelectionMade()
    {
        newSelectionMade = false;
    }

    public void SetTargetedEntity()
    {
        targeted_entity = highlighted_target;

    }

    public GameObject FindNearestEntity_withTag()
    {
        GameObject[] visible_entity_with_tag_array;

        visible_entity_with_tag_array = ScanForVisibleTargets();
        if (visible_entity_with_tag_array.Length > 0)
        {
            Array.Sort(visible_entity_with_tag_array, (a, b) =>
            {
                float distanceA = Vector3.Distance(transform.position, a.transform.position);
                float distanceB = Vector3.Distance(transform.position, b.transform.position);

                return distanceA.CompareTo(distanceB); // Sort in ascending order
            });

            return visible_entity_with_tag_array[0];
        }
        else
        {
            return null;
        }


    }

    public void PlotTargetArrow(GameObject target_obj)
    {
        _lineRenderer_targetArrow.enabled = true;
        _lineRenderer_targetArrow.startWidth = 0.5f;
        _lineRenderer_targetArrow.endWidth = 0.5f;
        _lineRenderer_targetArrow.positionCount = 3;
        _lineRenderer_targetArrow.material = character_lineMaterial;

        Vector3 obj_position = target_obj.transform.position;
        Vector3 tip_vertex = ((transform.position - obj_position).normalized * 0.5f);
        Vector3 left_vertex = Quaternion.AngleAxis(45f, Vector3.up) * (tip_vertex * 2f);
        Vector3 right_vertex = Quaternion.AngleAxis(-45f, Vector3.up) * (tip_vertex * 2f);
        _lineRenderer_targetArrow.SetPosition(0, left_vertex + obj_position + tip_vertex + Vector3.down * 0.5f);
        _lineRenderer_targetArrow.SetPosition(1, tip_vertex + obj_position + Vector3.down * 0.5f);
        _lineRenderer_targetArrow.SetPosition(2, right_vertex + obj_position + tip_vertex + Vector3.down * 0.5f);



    }

    public void DeactivateTargetArrow()
    {
        target_arrow_on = false;
        _lineRenderer_targetArrow.enabled = false;
    }

    public void ActivateTargetArrow()
    {
        if (!target_arrow_on)
        {
            target_arrow_on = true;
            _lineRenderer_targetArrow.enabled = true;
        }
        
    }

}



